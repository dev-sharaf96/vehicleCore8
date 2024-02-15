using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;

namespace TameenkDAL.Store
{
    public class QuotationRequestRepository : GenericRepository<QuotationRequest, int>
    {
        public QuotationRequestRepository(TameenkDbContext context)
            : base(context)
        {

        }

        public QuotationRequest GetQuotationRequestTreeByExternalId(string externalId)
        {
            var queyrableResult = DbSet.Include(q => q.Driver)
                    .Include(q => q.Drivers)
                    .Include(q => q.Drivers.Select(d => d.DriverLicenses))
                    .Include(q => q.Vehicle)
                    .Where(q => q.ExternalId == externalId);

            return queyrableResult.First();
        }

        /// <summary>
        /// Get user quotation by vehicle id .
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="vehicleIsRegistered"></param>
        /// <param name="showValidQuotationOnly">Show only the valid request that are not expired.</param>
        /// <returns></returns>
        public QuotationRequest GetUserQuotationByVehicleId(string vehicleId, bool vehicleIsRegistered, bool showValidQuotationOnly = true)
        {
            var query = DbSet.Where(x => (x.Vehicle.CustomCardNumber == vehicleId && !vehicleIsRegistered) || (x.Vehicle.SequenceNumber == vehicleId && vehicleIsRegistered));
            if (showValidQuotationOnly)
            {
                // Get only the valid request the are not expired within 16 hours window.
                var benchmarkDate = DateTime.Now.AddHours(-16);
                query = query.Where(qr => qr.CreatedDateTime > benchmarkDate);
            }
            return query.FirstOrDefault();
        }


        /// <summary>
        /// Get user quotation by vehicleId, main driver NIN, all additional drivers' NIN .and vehicleValue
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="vehicleIsRegistered"></param>
        /// <param name="mainDriverNin"></param>
        /// <param name="vehicleValue"></param>
        /// <param name="additionalDriversNins"></param>
        /// <returns></returns>
        public QuotationRequest GetQuotationRequestByDetails(string vehicleId, bool vehicleIsRegistered, string mainDriverNin, int vehicleValue, IEnumerable<string> additionalDriversNins)
        {
            var query = DbSet
                .Include(x => x.Driver)
                .Include(x => x.Drivers)
                .Include(x => x.Vehicle)
                .Where(x =>
                    (
                        (x.Vehicle.CustomCardNumber == vehicleId && !vehicleIsRegistered) ||
                        (x.Vehicle.SequenceNumber == vehicleId && vehicleIsRegistered)
                    ) &&
                    x.Driver != null && x.Driver.NIN == mainDriverNin &&
                    x.Vehicle != null && x.Vehicle.VehicleValue.HasValue && x.Vehicle.VehicleValue == vehicleValue);

            // Get only the valid request the are not expired within 16 hours window.
            var benchmarkDate = DateTime.Now.AddHours(-16);

            query = query.Where(qr => qr.CreatedDateTime > benchmarkDate);
            if (additionalDriversNins == null || !additionalDriversNins.Any())
            {
                return query.FirstOrDefault(q => !q.Drivers.Any());
            }

            foreach (var quotationRequest in query.ToList())
            {
                if (quotationRequest.Drivers != null && quotationRequest.Drivers.Any())
                {
                    var q = from dataNin in quotationRequest.Drivers.Select(d => d.NIN)
                            join nin in additionalDriversNins
                            on dataNin equals nin
                            select dataNin;
                    if (q.Count() == additionalDriversNins.Count() && q.Count() == quotationRequest.Drivers.Count)
                        return quotationRequest;
                }
            }
            //var quotationRequest = query.FirstOrDefault();
            //if (quotationRequest != null &&
            //    additionalDriversNins != null && additionalDriversNins.Any() &&
            //    quotationRequest.Drivers != null && quotationRequest.Drivers.Any())
            //{
            //    var q = from dataNin in quotationRequest.Drivers.Select(d => d.NIN)
            //            join nin in additionalDriversNins
            //            on dataNin equals nin
            //            select dataNin;
            //    if (q.Count() == additionalDriversNins.Count())
            //        return quotationRequest;
            //    else
            //        return null;
            //}
            return null;
        }

        /// <summary>
        /// Get Quotation Request obj by reference Id
        /// </summary>
        /// <param name="qtRqstExtrnlId"></param>
        /// <returns></returns>
        public QuotationRequest GetQuotationByExternalId(string qtRqstExtrnlId)
        {
            return DbSet.Where(x => x.ExternalId == qtRqstExtrnlId).FirstOrDefault();
        }

        public void UpdateIsComprehensiveRequestedStatus(bool status, string qtRqstExtrnlId)
        {
            QuotationRequest request = GetQuotationByExternalId(qtRqstExtrnlId);
            request.IsComprehensiveRequested = status;
            Update(request);
            Context.SaveChanges();
        }

        public void UpdateIsComprehensiveGeneratedStatus(bool status, string qtRqstExtrnlId)
        {
            QuotationRequest request = GetQuotationByExternalId(qtRqstExtrnlId);
            request.IsComprehensiveGenerated = status;
            Update(request);
            Context.SaveChanges();
        }
    }
}
