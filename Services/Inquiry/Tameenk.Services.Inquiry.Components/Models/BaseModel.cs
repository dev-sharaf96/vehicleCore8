using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;

namespace Tameenk.Services.Inquiry.Components
{
    public class BaseModel
    {
        public string Language { get; set; } = "ar";
        public Channel Channel { get; set; } =  Channel.Portal;
        public string MethodName { get; set; }
    }
}
