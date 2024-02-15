using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Cancellation.DAL.Repositories;
using Tameenk.Cancellation.DAL.Repositories.Interfaces;

namespace Tameenk.Cancellation.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        CancellationContext _context;

        ICancellationRequestRepository _CancellationRequests;
        IInsuranceCompanyRepository _InsuranceCompanies;
        IBankCodeRepository _BankCodes;
        IErrorCodeRepository _ErrorCodes;
        IReasonRepository _Reasons;
        IInsuranceTypeRepository _InsuranceTypes;
        IVehicleIDTypeRepository _VehicleIDTypes;
        IServiceRequestLogRepository _ServiceRequestLogs;
        public UnitOfWork(CancellationContext context)
        {
            _context = context;
        }

        public CancellationContext CancellationContext
        {
            set
            {
                _context = value;
            }
        }

        public ICancellationRequestRepository CancellationRequests
        {
            get
            {
                if (_CancellationRequests == null)
                    _CancellationRequests = new CancellationRequestRepository(_context);

                return _CancellationRequests;
            }
        }

        public IInsuranceCompanyRepository InsuranceCompanies
        {
            get
            {
                if (_InsuranceCompanies == null)
                    _InsuranceCompanies = new InsuranceCompanyRepository(_context);

                return _InsuranceCompanies;
            }
        }

        public IReasonRepository Reasons
        {
            get
            {
                if (_Reasons == null)
                    _Reasons = new ReasonRepository(_context);

                return _Reasons;
            }
        }


        public IBankCodeRepository BankCodes
        {
            get
            {
                if (_BankCodes == null)
                    _BankCodes = new BankCodeRepository(_context);

                return _BankCodes;
            }
        }
        public IErrorCodeRepository ErrorCodes
        {
            get
            {
                if (_ErrorCodes == null)
                    _ErrorCodes = new ErrorCodeRepository(_context);

                return _ErrorCodes;
            }
        }

        public IInsuranceTypeRepository InsuranceTypes
        {
            get
            {
                if (_InsuranceTypes == null)
                    _InsuranceTypes = new InsuranceTypeRepository(_context);

                return _InsuranceTypes;
            }
        }
        public IVehicleIDTypeRepository VehicleIDTypes
        {
            get
            {
                if (_VehicleIDTypes == null)
                    _VehicleIDTypes = new VehicleIDTypeRepository(_context);

                return _VehicleIDTypes;
            }
        }
        public IServiceRequestLogRepository ServiceRequestLogs
        {
            get
            {
                if (_ServiceRequestLogs == null)
                    _ServiceRequestLogs = new ServiceRequestLogRepository(_context);

                return _ServiceRequestLogs;
            }
        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        //public List<T> ExecSQL<T>(string query)
        //{
        //    return _context.ExecSQL<T>(query);
        //}
    }
}
