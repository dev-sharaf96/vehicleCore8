using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Profile.Component
{
   public class ConfirmModel
    {
        public string UserId { get; set; }
        public string Hashed { get; set; }
        public bool ConfirmPhone { get; set; }
        public bool ConfirmEmail { get; set; }
        public string Channel { get; set; }
    }
}
