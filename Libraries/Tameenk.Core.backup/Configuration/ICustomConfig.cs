using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Tameenk.Core.Configuration
{
    public interface ICustomConfig
    {
        void BuildConfig(XmlNode section);
    }
}
