using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.IdentityApi.Output.Models
{
    public class UserPromotionProgramModel
    {

        public string userId { get; set; }
        public string email { get; set; }
        public int programId { get; set; }
        public string channel { get; set; }
        public string language { get; set; }
    }
}