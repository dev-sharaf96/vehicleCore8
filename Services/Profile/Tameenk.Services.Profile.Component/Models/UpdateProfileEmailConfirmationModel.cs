using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Profile.Component
{
    public class UpdateProfileEmailConfirmationModel
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public DateTime RequestedDate { get; set; }
    }
}
