using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;

namespace Tameenk.Models.Checkout
{
    public class BaseModel
    {
        public string Language { get; set; } = "ar";
        public Channel Channel { get; set; } =  Channel.Portal;
    }
}
