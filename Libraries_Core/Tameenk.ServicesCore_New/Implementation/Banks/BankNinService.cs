using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Drivers;

namespace Tameenk.Services.Implementation
{
    public class BankNinService : IBankNinService
    {

        private readonly IRepository<BankNins> _bankNinRepository;


        public BankNinService(IRepository<BankNins> bankNinRepository)
        {
            _bankNinRepository = bankNinRepository ?? throw new TameenkArgumentNullException(nameof(bankNinRepository));
        }

        public List<string> GetBankNin(int? Bankid)
        {
            return _bankNinRepository.Table.Where(x => x.BankId == Bankid).Select(x => x.NIN).ToList();
        }

        public bool IsBankNinExist(int Bankid, string Nin)
        {
            return _bankNinRepository.Table.Any(x => x.BankId == Bankid && x.NIN == Nin);
        }

        public bool AddBankNin(int Bankid, List<string> Nin)
        {
            try
            {
                List<BankNins> bankNins = new List<BankNins>();

                foreach (var id in Nin)
                {
                    BankNins bank = new BankNins() { BankId = Bankid, NIN = id };
                    bankNins.Add(bank);

                }
                _bankNinRepository.Insert(bankNins);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteBankNin(int Bankid, List<string> bankNins)
        {
            try
            {
                List<BankNins> Nins = new List<BankNins>();
                foreach (var nin in bankNins)
                {
                    var bankNin = _bankNinRepository.Table.Where(x => x.NIN == nin && x.BankId == Bankid).FirstOrDefault();
                    Nins.Add(bankNin);

                }
                _bankNinRepository.Delete(Nins);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
