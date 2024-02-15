using System;
using System.Linq;
using System.Reflection;

namespace Tameenk.Services.Core
{
    public static class EnumSubTypeExtensions
    {
        public static bool IsEnumSubTypeOf(this IVRTicketSubTypeEnum ticketSubType, IVRTicketTypeEnum ticketType)
        {
            Type t = typeof(IVRTicketSubTypeEnum);
            MemberInfo mi = t.GetMember(ticketSubType.ToString()).FirstOrDefault(m => m.GetCustomAttribute(typeof(EnumSubType)) != null);
            if (mi == null)
                //throw new ArgumentException("Subcategory " + ticketSubType + " has no category.");
                return false;

            EnumSubType subAttr = (EnumSubType)mi.GetCustomAttribute(typeof(EnumSubType));
            return subAttr.TicketType == ticketType;
        }
    }
}
