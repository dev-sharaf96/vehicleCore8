using System;
using System.Data.Entity;
using System.Linq;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;

namespace TameenkDAL.Store
{
    public class QuotationResponseRepository : GenericRepository<QuotationResponse, int>
    {
        public QuotationResponseRepository(TameenkDbContext context)
            : base(context)
        {

        }

        public QuotationResponse GetQuotationResponse(string qtRqstExtrnlId, int insuranceTypeCode, bool vehicleAgencyRepair, short? deductibleValue)
        {
            return (DbSet.Include(q => q.QuotationRequest)
                    .Include(q => q.QuotationRequest.Driver)
                    .Include(q => q.QuotationRequest.Drivers)
                    .Include(q => q.QuotationRequest.Drivers.Select(d => d.DriverLicenses))
                    .Include(q => q.QuotationRequest.Vehicle)
                    .Include(q => q.Products)
                    .Include(q => q.Products.Select(x => x.Product_Benefits))
                    .Include(q => q.Products.Select(x => x.Product_Benefits.Select(y => y.Benefit)))
                    .Include(q => q.Products.Select(x => x.PriceDetails))
                    .Include(q => q.Products.Select(x => x.PriceDetails.Select(y => y.PriceType)))
                    .Where(
                        x => x.QuotationRequest.ExternalId == qtRqstExtrnlId &&
                        x.InsuranceTypeCode == insuranceTypeCode &&
                        (
                            (x.VehicleAgencyRepair.HasValue && x.VehicleAgencyRepair.Value == vehicleAgencyRepair) ||
                            (!vehicleAgencyRepair && !x.VehicleAgencyRepair.HasValue)
                        ) &&
                        (
                            (!deductibleValue.HasValue && !x.DeductibleValue.HasValue) ||
                            (deductibleValue.HasValue && x.DeductibleValue.HasValue && x.DeductibleValue.Value == deductibleValue.Value)
                        ))).ToList().FirstOrDefault(y => IsGivenDateWithin16Hours(y.CreateDateTime));
        }

        public QuotationResponse GetQuotationResponseAttachedWithQuoutationRequestAndDrivers(string referenceId)
        {
            var quotationResponses = DbSet.Include(q => q.QuotationRequest)
                    .Include(q => q.QuotationRequest.Driver)
                    .Include(q => q.QuotationRequest.Drivers)
                    .Where(q => q.ReferenceId == referenceId);

            return quotationResponses.FirstOrDefault();
        }

        public short? GetInsuranceTypeCodeByProductId(Guid productId)
        {
            return DbSet
                    .Where(q => q.Products
                                .Any(p => p.Id == productId))
                    .Select(q => q.InsuranceTypeCode)
                    .FirstOrDefault();
        }

        private bool IsGivenDateWithin16Hours(DateTime givenDate)
        {
            return DateTime.Now.Subtract(givenDate).TotalHours < 16;
        }
    }
}
