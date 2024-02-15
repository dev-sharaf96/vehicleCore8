using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Leasing.Models
{
   public class LeasingClientPolicyModel
    {
        public string ReferenceId { get; set; }
        public Guid  SelectedProductId { get; set; }
    }
}
