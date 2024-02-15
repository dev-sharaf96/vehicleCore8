using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Extensions
{
    public static class GuidExtension
    {
        public static string ConvertToBase16(this Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray()).Replace("/", "")
            .Replace("+", "")
            .Replace("=", "");
        }

    }
}
