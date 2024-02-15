using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Implementation.Orders
{
    public class UserBank
    {
        public string IBAN { get; set; }

        public string Code { get; set; }

        public string EnglishDescription { get; set; }

        public string ArabicDescription { get; set; }

        public int? ValidationCode { get; set; }

    }
}
