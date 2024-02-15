using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Resources
{
    public class QueryResources
    {
        public string GetValue(System.Resources.ResourceManager resourceManager, string key,string language)
        {
           var result= resourceManager.GetString(key, CultureInfo.GetCultureInfo(language));
           return result;
        }
    }
}
