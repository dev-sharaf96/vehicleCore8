using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.IdentityApi.Models
{
    public class ConfirmJoinProgramModel
    {
        //public string Token { get; set; }
        public string Key { get; set; }
        public string Hashed { get; set; }
        public string Channel { get; set; }
        public string Lang { get; set; }
    }
}