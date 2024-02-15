using System.Collections.Generic;
using System.Linq;
using Tameenk.Core.Domain.Enums;
using TameenkDAL.Store;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public class BankLookup : IBankLookup
    {
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> banks = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        
        private static BankLookup bankLookup = null;
        private BankLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static BankLookup Instance
        {
            get
            {
                if(bankLookup == null)
                {
                    bankLookup = new BankLookup();
                }
                return bankLookup;
            }
        }

        public List<Lookup> GetBanks(LanguageTwoLetterIsoCode language)
        {
            if (banks.ContainsKey(language))
                return banks[language];
            banks[language] = lookupsDAL.GetAllBankCodes().Select(a => new Lookup
            {
                Id = a.Code,
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.ArabicDescription : a.EnglishDescription
            }).ToList();

            return banks[language];
        }

    }
}
