using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;

namespace Tameenk.Core.Domain.Dtos
{
    public class BaseViewModel
    {
        //public string Channel { get; set; } = "web";
        //public string Language { get; set; } = "en";

        public string Language { get; set; } = "ar";
        public Channel Channel { get; set; } = Channel.Portal;
    }
}
