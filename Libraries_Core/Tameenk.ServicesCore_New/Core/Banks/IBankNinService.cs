using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Services.Implementation.Drivers;

namespace Tameenk.Services.Core
{
    public interface IBankNinService
    {

        List<string> GetBankNin(int ?Bankid);
        bool IsBankNinExist(int Bankid, string Nin);
        bool AddBankNin(int Bankid, List<string> Nin);
        bool DeleteBankNin(int Bankid, List<string> Nin);

    }
}
