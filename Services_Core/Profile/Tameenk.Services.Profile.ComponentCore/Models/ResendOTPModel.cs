using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Profile.Component
{
    public class ResendOTPModel : BaseViewModel
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string NationalId { get; set; }
    }
}
