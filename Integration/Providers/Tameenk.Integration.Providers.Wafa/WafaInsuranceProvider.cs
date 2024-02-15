using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Tameenk.Core.Configuration;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;

namespace Tameenk.Integration.Providers.Wafa
{
    public class WafaInsuranceProvider : InsuranceProvider
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _tameenkConfig;
        #endregion

        #region Ctor
        public WafaInsuranceProvider(TameenkConfig tameenkConfig,ILogger logger)
             : base(new ProviderConfiguration() { ProviderName = "Wafa" }, logger)
        {
            _logger = logger;
            _tameenkConfig = tameenkConfig;
        }
        #endregion

        #region methods
        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            try
            {
                string nameSpace = "Tameenk.Integration.Providers.Wafa";




                // using Tameenk config 
                var testMode = _tameenkConfig.Quotatoin.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.quotationTestData.json";

                    string data = ReadResource(nameSpace, nameOfFile);

                    WafaService.MotorQuotationRequestOutput response = JsonConvert.DeserializeObject<WafaService.MotorQuotationRequestOutput>(data);
                    return response;
                }



                WafaService.IntegrationClient serviceClient = new WafaService.IntegrationClient("BasicHttpBinding_IIntegration");
                WafaService.MotorQuotationRequestInput val = new WafaService.MotorQuotationRequestInput();
                val.BrokerReferenceNo = quotation.ReferenceId;
                val.PolicyType = quotation.ProductTypeCode == 2 ? WafaService.PolicyType.Comprehensive : WafaService.PolicyType.TPL; // TPL or Comprehensive
                val.ExpectedEffectiveDate = DateTime.Now.AddDays(2).Date;
                val.IsOwnerDriver = false;
                val.Authenticate = new WafaService.Authenticate()
                {
                    BrokerCode = Config.WafaaBrokerCode,
                    Password = Config.WafaaPassword
                };
                val.OwnerInformation = new WafaService.OwnerInformation()
                {
                    PolicyHolderID = Convert.ToInt64(quotation.InsuredId),
                    PolicyHolderFirstName = quotation.InsuredFirstNameEn,
                    PolicyHolderSecondName = quotation.InsuredMiddleNameEn,
                    PolicyHolderThirdName = quotation.InsuredLastNameEn
                };
                if (quotation.InsuredIdTypeCode == 1)
                {
                    val.OwnerInformation.PolicyHolderIdentityTypeCode = WafaService.IdentityType.NationalId;
                    val.OwnerInformation.DateOfBirthH = quotation.InsuredBirthDate;
                }
                else if (quotation.InsuredIdTypeCode == 2)
                {
                    val.OwnerInformation.PolicyHolderIdentityTypeCode = WafaService.IdentityType.Iqama;
                    val.OwnerInformation.DateOfBirthG = DateTime.ParseExact(quotation.InsuredBirthDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                var DriverDetails = new WafaService.DriverDetails[quotation.Drivers.Count];
                for (int i = 0; i < quotation.Drivers.Count; i++)
                {
                    DriverDetails[i] = new WafaService.DriverDetails()
                    {
                        DriverType = i == 0 ? WafaService.DriverType.Primary : WafaService.DriverType.Secondary, // Primary or Secondary
                        DriverId = quotation.Drivers[i].DriverId,
                        DriverDOBG = quotation.Drivers[i].DriverBirthDateG,//DateTime.ParseExact(quotation.Drivers[i].DriverBirthDateG.Replace("/", "-"), "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        DriverName = quotation.Drivers[i].DriverFirstNameEn
                    };
                }

                if (DriverDetails.Length == 0)
                    DriverDetails = null;

                CultureInfo arSA = new CultureInfo("ar-SA");
                arSA.DateTimeFormat.Calendar = new UmAlQuraCalendar();
                val.VehicleInformation = new WafaService.VehicleInformation()
                {
                    VehicleType = string.IsNullOrEmpty(quotation.VehicleRegExpiryDate) ? WafaService.VehicleType.Custom : WafaService.VehicleType.Registered,
                    VehicleSequenceNumber = quotation.VehicleId,
                    ManufactureYear = quotation.VehicleModelYear,
                    VehicleDriveCityID = int.Parse(quotation.InsuredCityCode),
                    VehicleMakeCode =  int.Parse(quotation.VehicleMakerCode),
                    VehicleModelCode = quotation.VehicleModelCode,
                    NCDFreeYears = quotation.NCDFreeYears,
                    VehicleMake = quotation.VehicleMaker,
                    VehicleModel = quotation.VehicleModel,
                    VehicleColor = quotation.VehicleMajorColor,
                    VehicleRegisterationExpiryDate = string.IsNullOrEmpty(quotation.VehicleRegExpiryDate) ? default(DateTime?) : DateTime.ParseExact(quotation.VehicleRegExpiryDate, "dd-MM-yyyy",
                       arSA),
                    DriverDetails = DriverDetails,
                    ChasisNumber = quotation.VehicleChassisNumber,
                    FirstPlateLetterID = quotation.VehiclePlateText1,
                    SecondPlateLetterID = quotation.VehiclePlateText2,
                    ThirdPlateLetterID = quotation.VehiclePlateText3,
                    VehiclePlateNumber = quotation.VehiclePlateNumber,
                    VehicleColorCode = quotation.VehicleMajorColorCode != null ? int.Parse(quotation.VehicleMajorColorCode) : 0
                };

                System.Globalization.DateTimeFormatInfo DTFormat = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                DTFormat.Calendar = new System.Globalization.GregorianCalendar();
                DTFormat.ShortDatePattern = "dd/MM/yyyy";
                if (val.VehicleInformation.VehicleRegisterationExpiryDate.HasValue)
                {
                    val.VehicleInformation.VehicleRegisterationExpiryDate = DateTime.ParseExact(val.VehicleInformation.VehicleRegisterationExpiryDate.Value.Date.ToString("d", DTFormat),
                        "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                // only for Comprehensive
                if (quotation.ProductTypeCode == 2)
                {
                    val.VehicleInformation.AgencyCover = WafaService.AgencyCover.Inside;
                    val.VehicleInformation.SumInsured = quotation.VehicleValue.Value;
                    val.VehicleInformation.Deductible = quotation.DeductibleValue.Value; // DeductibleId no value, we need maping with Deductible value
                }

                WafaService.MotorQuotationRequestOutput responseData = serviceClient.GenerateMotorQuotation(val);
                return responseData;
            }
            catch (Exception ex)
            {
                _logger.Log($"WafaInsuranceProvider -> ExecuteQuotationRequest", ex, LogLevel.Error);
            }

            return null;
        }

        protected override QuotationServiceResponse GetQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            QuotationServiceResponse responseValue = new QuotationServiceResponse();
            // dynamic responseData = response;
            //var responseData = JsonConvert.DeserializeObject<WafaService.MotorQuotationRequestOutput>(response.ToString());
            try
            {
                var result = new JavaScriptSerializer().Serialize(response);
                var responseData = JsonConvert.DeserializeObject<WafaService.MotorQuotationRequestOutput>(result);
                if (responseData.Status == WafaService.Status.Failure)
                {
                    responseValue.StatusCode = 2;
                    responseValue.Errors = new List<Error>();
                    foreach (var error in responseData.Errors)
                        responseValue.Errors.Add(new Error { Message = error.ErrorMessage, Code = error.ErrorCode.ToString() });
                }
                else
                {
                    responseValue.StatusCode = 1;
                    responseValue.Products = new List<ProductDto>();
                    responseValue.Products.Add(new ProductDto()
                    {
                        ProductPrice = responseData.Premium.GrossPremium + responseData.Premium.VAT + responseData.Premium.Fees - (responseData.Premium.Discount + responseData.Premium.NCDDiscount)
                    });
                    responseValue.QuotationNo = responseData.QuotationNumber;
                    responseValue.QuotationExpiryDate = responseData.QuotationExpiryDate.ToString();
                }
            }

            catch (Exception ex)
            {
                _logger.Log($"WafaInsuranceProvider -> GetQuotationResponseObject", ex, LogLevel.Error);
                responseValue.StatusCode = 2;
                if (responseValue.Errors == null)
                    responseValue.Errors = new List<Error>();
                responseValue.Errors.Add(new Error { Message = ex.GetBaseException().Message });
            }
            return responseValue;
        }
        private string ReadResource(string strnamespace, string strFileName)
        {

            try
            {

                var assembly = Assembly.Load(strnamespace);
                string result = "";
                Stream stream = assembly.GetManifestResourceStream(strnamespace + strFileName);

                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            try
            {
                string nameSpace = "Tameenk.Integration.Providers.Wafa";


                // using Tameenk config 
                var testMode = _tameenkConfig.Policy.TestMode;
                if (testMode)
                {
                                    
                    const string nameOfFileJson = ".TestData.policyTestData.json";

                    string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
                    XmlSerializer serializer = new XmlSerializer(typeof(WafaService.MotorPolicyConversionRequestOutput));
                    WafaService.MotorPolicyConversionRequestOutput Policyresponse = new WafaService.MotorPolicyConversionRequestOutput();
                    Policyresponse.ByteArray = Encoding.UTF8.GetBytes(responseDataJson);
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            serializer.Serialize(writer, Policyresponse);
                            return Policyresponse;

                        }
                    }
                   
                }



                WafaService.IntegrationClient serviceClient = new WafaService.IntegrationClient();
                WafaService.MotorPolicyConversionRequestInput val = new WafaService.MotorPolicyConversionRequestInput();
                val.Authenticate = new WafaService.Authenticate()
                {
                    BrokerCode = Config.WafaaBrokerCode,
                    Password = Config.WafaaPassword
                };

                val.BrokerReferenceNo = policy.ReferenceId;
                val.QuotationNumber = policy.QuotationNo;
                val.ReportType = WafaService.ReportType.GenerateByteArray;
                val.AddressDetails = new WafaService.AddressDetails()
                {
                    BuildingNumber = policy.InsuredBuildingNo,
                    PoBox = policy.InsuredZipCode,
                    UnitNumber = policy.InsuredUnitNo,
                    ZipCode = policy.InsuredAdditionalNumber,
                    District = policy.InsuredDistrict,
                    //City = int.Parse(policy.InsuredCityCode), // send same value in quotation.InsuredCityCode
                    Street = policy.InsuredStreet,
                    Address = policy.InsuredStreet,
                };
                val.OwnerInformation = new WafaService.OwnerInformation()
                {
                    EmailId = policy.InsuredEmail,
                    //MobileNumber = long.Parse(policy.InsuredMobileNumber), // mobile format 05xxxxxxxx
                    //PolicyHolderFirstName = policy.InsuredFirstNameAr,
                    //PolicyHolderSecondName = policy.InsuredMiddleNameAr,
                    //PolicyHolderThirdName = policy.InsuredLastNameAr
                };
                val.VehicleInformationConversion = new WafaService.VehicleInformationConversion()
                {
                  //  ChasisNumber = policy.VehicleChassisNumber,
                };
                // only if VehicleType = Registered 
                //if (policy.VehicleIdTypeCode == 1)
                //{
                //    val.VehicleInformationConversion.VehiclePlateNumber = policy.VehiclePlateNumber;
                //    val.VehicleInformationConversion.FirstPlateLetterID = policy.VehiclePlateText1;
                //    val.VehicleInformationConversion.SecondPlateLetterID = policy.VehiclePlateText2;
                //    val.VehicleInformationConversion.ThirdPlateLetterID = policy.VehiclePlateText3;
                //}
                WafaService.MotorPolicyConversionRequestOutput responseData = serviceClient.ConvertMotorQuoteToPolicy(val);
                return responseData;
                
            }
            catch (Exception ex)
            {
                _logger.Log($"WafaInsuranceProvider -> ExecutePolicyRequest", ex, LogLevel.Error);
            }

            return null;
        }

        protected override PolicyResponse GetPolicyResponseObject(object response, PolicyRequest request = null)
        {
            try
            {
                var result = new JavaScriptSerializer().Serialize(response);
                var responseData = JsonConvert.DeserializeObject<WafaService.MotorPolicyConversionRequestOutput>(result);

                PolicyResponse policyResponse = new PolicyResponse();
                if (responseData.Status == WafaService.Status.Failure)
                {
                    policyResponse.StatusCode = 2;
                    policyResponse.Errors = new List<Error>();
                    foreach (var error in responseData.Errors)
                    {
                        policyResponse.Errors.Add(new Error { Message = error.ErrorMessage, Code = error.ErrorCode.ToString() });
                    }
                }
                else
                {
                    policyResponse.PolicyNo = responseData.PolicyNumber;
                    policyResponse.StatusCode = 1;
                    policyResponse.PolicyFile = responseData.ByteArray;

                }
                return policyResponse;
            }
            catch (Exception ex)
            {
                _logger.Log("", ex, LogLevel.Error);
            }

            return null;
        }

        protected override ProviderInfoDto GetProviderInfo()
        {
            var providerInfo = new ProviderInfoDto
            {
                QuotationUrl = "http://uateai.wafainsurance.com/BrokerIntegration/Integration.svc/GenerateMotorQuotation",
                PolicyUrl = "http://uateai.wafainsurance.com/BrokerIntegration/Integration.svc/ConvertMotorQuoteToPolicy"
            };
            return providerInfo;
        }

        //public override PolicyResponse GetPolicy(PolicyRequest policy)
        //{
        //    WafaService.IntegrationClient serviceClient = new WafaService.IntegrationClient();
        //    WafaService.MotorPolicyConversionRequestInput val = new WafaService.MotorPolicyConversionRequestInput();
        //    val.Authenticate = new WafaService.Authenticate()
        //    {
        //        BrokerCode = Config.WafaaBrokerCode,
        //        Password = Config.WafaaPassword
        //    };

        //    val.BrokerReferenceNo = policy.ReferenceId;
        //    val.QuotationNumber = policy.QuotationNo;
        //    val.ReportType = WafaService.ReportType.GenerateByteArray;
        //    val.AddressDetails = new WafaService.AddressDetails()
        //    {
        //        BuildingNumber = policy.InsuredBuildingNo,
        //        PoBox = policy.InsuredZipCode,
        //        UnitNumber = policy.InsuredUnitNo,
        //        ZipCode = policy.InsuredAdditionalNumber,
        //        District = policy.InsuredDistrict,
        //        City = int.Parse(policy.InsuredCityCode), // send same value in quotation.InsuredCityCode
        //        Street = policy.InsuredStreet,
        //        Address = policy.InsuredStreet,
        //    };
        //    val.OwnerInformation = new WafaService.OwnerInformation()
        //    {
        //        EmailId = policy.InsuredEmail,
        //        MobileNumber = long.Parse(policy.InsuredMobileNumber), // mobile format 05xxxxxxxx
        //        PolicyHolderFirstName = policy.InsuredFirstNameAr,
        //        PolicyHolderSecondName = policy.InsuredMiddleNameAr,
        //        PolicyHolderThirdName = policy.InsuredLastNameAr
        //    };
        //    val.VehicleInformationConversion = new WafaService.VehicleInformationConversion()
        //    {
        //        ChasisNumber = policy.VehicleChassisNumber,
        //    };
        //    // only if VehicleType = Registered 
        //    if (policy.VehicleIdTypeCode == 1)
        //    {
        //        val.VehicleInformationConversion.VehiclePlateNumber = policy.VehiclePlateNumber;
        //        val.VehicleInformationConversion.FirstPlateLetterID = policy.VehiclePlateText1;
        //        val.VehicleInformationConversion.SecondPlateLetterID = policy.VehiclePlateText2;
        //        val.VehicleInformationConversion.ThirdPlateLetterID = policy.VehiclePlateText3;
        //    }
        //    WafaService.MotorPolicyConversionRequestOutput responseData = serviceClient.ConvertMotorQuoteToPolicy(val);

        //    PolicyResponse policyResponse = new PolicyResponse();
        //    if (responseData.Status == WafaService.Status.Failure)
        //    {
        //        policyResponse.StatusCode = 2;
        //        policyResponse.Errors = new List<Error>();
        //        foreach (var error in responseData.Errors)
        //        {
        //            policyResponse.Errors.Add(new Error { Message = error.ErrorMessage, Code = error.ErrorCode.ToString() });
        //        }
        //    }
        //    else
        //    {
        //        policyResponse.PolicyNo = responseData.PolicyNumber;
        //        policyResponse.StatusCode = 1;
        //        policyResponse.PolicyFile = responseData.ByteArray;

        //    }
        //    return policyResponse;
        //}
        #endregion
        protected override object ExecuteVehicleClaimRegistrationRequest(ClaimRegistrationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            ClaimRegistrationServiceOutput output = SubmitVehicleClaimRegistrationRequest(claim, predefinedLogInfo);
            if (output.ErrorCode != ClaimRegistrationServiceOutput.ErrorCodes.Success)
                return null;

            return output.Output;
        }

        protected override object ExecuteVehicleClaimNotificationRequest(ClaimNotificationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            ClaimNotificationServiceOutput output = SubmitVehicleClaimNotificationRequest(claim, predefinedLogInfo);
            if (output.ErrorCode != ClaimNotificationServiceOutput.ErrorCodes.Success)
                return null;

            return output.Output;
        }
        protected override object ExecuteAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitAutoleasingQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
        }

        protected override object ExecuteAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitAutoleasingPolicyRequest(policy, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
        }
    }
}
