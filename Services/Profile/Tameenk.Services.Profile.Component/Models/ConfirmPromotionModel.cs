using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Profile.Component
{
    public class ConfirmPromotionModel : BaseViewModel
    {
        public string Key { get; set; }
        public string Hashed { get; set; }
        public string Channel { get; set; }
        public string Lang { get; set; }
        public int JoinTypeId { get; set; }
    }
}
