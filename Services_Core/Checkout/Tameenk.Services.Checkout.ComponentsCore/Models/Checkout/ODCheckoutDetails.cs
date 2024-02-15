using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Checkout.Components
{
    public class ODCheckoutDetails
    {
        public String ReferenceId { get; set; }
        public string ProductId { get; set; }
        public List<long> SelectedProductBenfitId { get; set; }
    }
}
