using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Services.Core.Quotations;
using TameenkDAL.UoW;
using Tamkeen.bll;
using Tamkeen.bll.Lookups;
using Tamkeen.bll.Model;
using Tamkeen.Utilities;


namespace Tameenk.Controllers
{
    public class QuotationController : Controller
    {
        readonly DeductibleLookup _deductible;
        private readonly IQuotationService _quotationService;
        readonly ITameenkUoW _tameenkUoW;

        public QuotationController(ITameenkUoW tameenkUoW, IQuotationService quotationService)
        {
            _tameenkUoW = tameenkUoW;
            _deductible = new DeductibleLookup();
            _quotationService = quotationService;
        }


        public ActionResult GetCompanyMoreInfo(int iCompanyId)
        {
            var matchedComp = _tameenkUoW.InsuranceCompanyRepository.GetWithInclude(comp => comp.InsuranceCompanyID == iCompanyId, "Contact", "Address").FirstOrDefault();
            if (matchedComp != null)
                return Json(new
                {
                    DescriptionAr = matchedComp.DescAR,
                    DescriptionEn = matchedComp.DescEN,
                    Email = matchedComp.Contact != null ? matchedComp.Contact.Email : "",
                    Fax = matchedComp.Contact != null ? matchedComp.Contact.Fax : "",
                    HomePhone = matchedComp.Contact != null ? matchedComp.Contact.HomePhone : "",
                    MobileNumber = matchedComp.Contact != null ? matchedComp.Contact.MobileNumber : "",
                    AddressLoction = matchedComp.Address != null ? matchedComp.Address.AddressLoction : "",

                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Error = true }, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> SearchResult(string qtRqstExtrnlId, int TypeOfInsurance = 1, bool VehicleAgencyRepair = false, short DeductibleValue = 0)
        {
            if (!ValidateSearchResult(qtRqstExtrnlId, TypeOfInsurance, VehicleAgencyRepair, DeductibleValue))
            {
                return RedirectToAction("Index", "Home");
            }
            var quotationRequest = _quotationService.GetQuotationRequest(qtRqstExtrnlId);
            if (quotationRequest != null && quotationRequest.CreatedDateTime < DateTime.Now.AddHours(-16))
            {
                return RedirectToAction("Index", "Error", new { key = "QuotationExpired" });
            }
            //Need to be generic
            //when typeOfInsurance  =  2 then check if there a request to companyies with this search criteria is requested and generatead in the db or not
            //if generated then will return the data from db
            //else then request the companies and save the result into db
            if (TypeOfInsurance == 2 && DeductibleValue == 2000)
            {
                // _quotationRequestServices.WaitUntilComprehensiveQuotsGenerated(qtRqstExtrnlId);
            }

            //if any company return errors we save dumy product into the db and set request time before 16hr
            bool saveResponseToDbWithOldDate = false;
            bool.TryParse(ConfigurationManager.AppSettings["ShowInsuranceCompanyErrors"], out saveResponseToDbWithOldDate);
            var result = BuildQuotationResponseModel(_quotationService.GetQuotationRequestDrivers(qtRqstExtrnlId), TypeOfInsurance, VehicleAgencyRepair, DeductibleValue == 0 ? null : (short?)DeductibleValue, saveResponseToDbWithOldDate);

            ViewBag.DeductibleValuesList = _deductible.GetDeductibleValuesList();
            //if insurance company is 1 set the deductible value = 1500 so that make it the default selected in the drpdown of deductibleValues
            if (TypeOfInsurance == 1)
            {
                result.DeductibleValue = (short?)Config.DefaultDeductibleValue;
            }
            if (TempData.ContainsKey("Checkout_HomePageErrors"))
            {
                var errors = TempData["Checkout_HomePageErrors"] as List<string>;
                if (errors != null)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError($"CheckoutError", error);
                    }
                }
            }

            return View(result);
        }

        public QuotationResponseModel BuildQuotationResponseModel(QuotationRequest quotationRequest, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, short? deductibleValue = null, bool saveQuotResponseWithOldDate = false)
        {
            var result = new QuotationResponseModel();

            result.QtRqstExtrnlId = quotationRequest.ExternalId;
            result.Vehicle = new VehicleModel();

            result.Vehicle.Id = quotationRequest.Vehicle.ID;
            result.Vehicle.Maker = quotationRequest.Vehicle.VehicleMaker;
            result.Vehicle.MakerCode = quotationRequest.Vehicle.VehicleMakerCode.HasValue ? quotationRequest.Vehicle.VehicleMakerCode.Value : default(short);
            result.Vehicle.Model = quotationRequest.Vehicle.VehicleModel;
            result.Vehicle.ModelYear = quotationRequest.Vehicle.ModelYear;
            result.Vehicle.PlateTypeCode = quotationRequest.Vehicle.PlateTypeCode;
            result.Vehicle.PlateColor = CarPlateUtils.GetCarPlateColorByCode((int?)quotationRequest.Vehicle.PlateTypeCode);
            result.Vehicle.CarPlate = new CarPlateInfo(quotationRequest.Vehicle.CarPlateText1,
            quotationRequest.Vehicle.CarPlateText2, quotationRequest.Vehicle.CarPlateText3,
            quotationRequest.Vehicle.CarPlateNumber.HasValue ? quotationRequest.Vehicle.CarPlateNumber.Value : 0);

            result.NCDFreeYears = _quotationService.GetNCDFreeYearsDescription(quotationRequest.NajmNcdFreeYears.Value, CultureInfo.CurrentCulture.DisplayName);
            result.TypeOfInsurance = insuranceTypeCode;
            result.TypeOfInsuranceText = insuranceTypeCode == 1 ? Langbll.Tpl_txt : Langbll.Comprehensive_txt;
            result.DeductibleValue = deductibleValue;
            result.VehicleAgencyRepair = vehicleAgencyRepair;

            return result;
        }
        public ActionResult GetQuotationErrors(List<string> errors)
        {
            return PartialView("_QuotationErrors", errors);
        }

        #region Private Methods


        private Dictionary<string, string> GetModelKeyValueErrors()
        {
            var errorList = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                );
            return errorList;
        }


        /// <summary>
        /// Check if there are drivers ids and create Driver list and add them to request Message
        /// </summary>
        /// <param name="requestMessage"></param>
        private void AddDriversToRequestMessage(QuotationServicesApiRequestMessage requestMessage)
        {
            if (requestMessage.DriverId != null)
            {
                List<Tamkeen.bll.Model.DriverModel> DriverList = new List<Tamkeen.bll.Model.DriverModel>();
                for (int i = 0; i < requestMessage.DriverId.Length; i++)
                {
                    if (requestMessage.DriverId[i] != null)
                    {
                        Tamkeen.bll.Model.DriverModel driver = new Tamkeen.bll.Model.DriverModel();
                        driver.DriverId = (long)requestMessage.DriverId[i];
                        driver.LicenseExpirationYear = requestMessage.DriverLicenseExpirationYear[i];
                        driver.DriverlicenseExpiryDate = requestMessage.DriverlicenseExpiryDate[i];
                        //add driver obj to the list
                        DriverList.Add(driver);
                    }
                }
                requestMessage.Drivers = DriverList;
            }
        }

        /// <summary>
        /// Validate Search Result action input
        /// </summary>
        /// <param name="qtRqstExtrnlId"></param>
        /// <param name="TypeOfInsurance"></param>
        /// <param name="VehicleAgencyRepair"></param>
        /// <param name="DeductibleValue"></param>
        /// <returns></returns>
        private bool ValidateSearchResult(string qtRqstExtrnlId, int TypeOfInsurance, bool VehicleAgencyRepair, short DeductibleValue)
        {
            if (string.IsNullOrEmpty(qtRqstExtrnlId))
            {
                return false;
            }
            if (TypeOfInsurance < 0 || TypeOfInsurance > 2)
                return false;
            if (DeductibleValue < 0)
                return false;
            return true;
        }

        #endregion
    }
}