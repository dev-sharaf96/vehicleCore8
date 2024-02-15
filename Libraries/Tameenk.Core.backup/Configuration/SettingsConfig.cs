using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    public class SettingsConfig
    {
        public SettingsConfig(XmlNode section)
        {
            var settingSection = section.SelectSingleNode("Settings");
            Path = settingSection.GetString("Path");
            LogConfigPath = settingSection.GetString("LogConfigPath");
            AdminPath = settingSection.GetString("AdminPath");
        }

        public string Path { get; private set; }
        public string AdminPath { get; private set; }

        public string LogConfigPath { get; private set; }
    }
}
