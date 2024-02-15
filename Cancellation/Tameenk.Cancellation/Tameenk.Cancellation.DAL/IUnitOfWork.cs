using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Cancellation.DAL.Repositories.Interfaces;

namespace Tameenk.Cancellation.DAL
{
    public interface IUnitOfWork
    {
        CancellationContext CancellationContext { set; }

        ICancellationRequestRepository CancellationRequests { get; }
        IInsuranceCompanyRepository InsuranceCompanies { get; }

        IBankCodeRepository BankCodes { get; }
        IReasonRepository Reasons { get; }
        IErrorCodeRepository ErrorCodes { get; }
        IInsuranceTypeRepository InsuranceTypes { get; }
        IVehicleIDTypeRepository VehicleIDTypes { get; }
        IServiceRequestLogRepository ServiceRequestLogs { get; }
        //   List<T> ExecSQL<T>(string query);
        int SaveChanges();
    }
}
