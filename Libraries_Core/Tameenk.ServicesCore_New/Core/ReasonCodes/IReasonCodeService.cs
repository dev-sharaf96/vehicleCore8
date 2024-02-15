using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Core.ReasonCodes
{
    public interface IReasonCodeService
    {

        List<ReasonCode> GetAll(string lang);
    }
}
