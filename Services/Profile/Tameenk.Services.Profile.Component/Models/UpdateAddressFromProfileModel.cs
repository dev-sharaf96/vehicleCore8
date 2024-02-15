using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Profile.Component
{
    public class UpdateAddressFromProfileModel
    {
        //public Address Address { get; set; }
        public string NationalId { get; set; }
        public string Lang { get; set; }
        public string Channel { get; set; }
    }
}
