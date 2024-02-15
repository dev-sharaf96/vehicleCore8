using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Tameenk.Core.Extensions
{
    public static class TameenkConfigExtension
    {
        public static T SetByXElement<T>(this XmlNode node, string attrName, Func<string, T> converter)
        {
            if (node == null || node.Attributes == null) return default(T);
            var attr = node.Attributes[attrName];
            if (attr == null) return default(T);
            var attrVal = attr.Value;
            return converter(attrVal);
        }

        public static string GetString(this XmlNode node, string attrName)
        {
            return node.SetByXElement<string>(attrName, Convert.ToString);
        }

        public static int GetInteger(this XmlNode node, string attrName)
        {
            return node.SetByXElement<int>(attrName, Convert.ToInt32);
        }

        public static bool GetBool(this XmlNode node, string attrName)
        {
            return node.SetByXElement<bool>(attrName, Convert.ToBoolean);
        }
    }
}
