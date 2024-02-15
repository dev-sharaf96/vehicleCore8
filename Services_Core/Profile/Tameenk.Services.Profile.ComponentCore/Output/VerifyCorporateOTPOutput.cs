using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using TameenkDAL.Models;

namespace Tameenk.Services.Profile.Component.Output
{
    public class VerifyCorporateOTPOutput
    {
        public AccessTokenResult AccessTokenResult { get; set; }
        public CorporateUsers CorporateUser { get; set; }
    }
}
