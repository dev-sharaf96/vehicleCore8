using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumSubType : Attribute
    {
        public EnumSubType(IVRTicketTypeEnum type)
        {
            TicketType = type;
        }
        public IVRTicketTypeEnum TicketType { get; private set; }
    }
}
