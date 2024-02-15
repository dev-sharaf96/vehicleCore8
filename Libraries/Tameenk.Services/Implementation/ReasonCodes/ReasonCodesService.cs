using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core.ReasonCodes;

namespace Tameenk.Services.Implementation.ReasonCodes
{

    public class ReasonCodesService : IReasonCodeService
    {
        private readonly IRepository<ReasonCode> _reasonCodesRepository;

        public ReasonCodesService(IRepository<ReasonCode> reasonCodesRepository)
        {
            this._reasonCodesRepository = reasonCodesRepository;


        }
        public List<ReasonCode> GetAll(string lang="en")
        {

            return _reasonCodesRepository.TableNoTracking.ToList();

        }
    }
}
