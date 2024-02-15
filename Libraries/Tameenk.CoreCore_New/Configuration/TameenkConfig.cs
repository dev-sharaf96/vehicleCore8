using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    /// <summary>
    /// Represents startup Tameenk configuration parameters
    /// </summary>
    public class TameenkConfig : IConfigurationSectionHandler
    {

        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new TameenkConfig();

            var startupNode = section.SelectSingleNode("Startup");
            config.IgnoreStartupTasks = startupNode.GetBool("IgnoreStartupTasks");


            var webFarmsNode = section.SelectSingleNode("WebFarms");
            config.MultipleInstancesEnabled = webFarmsNode.GetBool("MultipleInstancesEnabled");
            config.RunOnAzureWebsites = webFarmsNode.GetBool("RunOnAzureWebsites");


            #region Create SMTP Obj
            config.SMTP = CreateSMTPObj(section);

            #endregion

            config.Policy = new PolicyConfig(section);

            #region PayFortConfig

            config.PayFort = new PayFortConfig(section);

            #endregion

            config.Quotatoin = new QuotatoinConfig(section);

            config.HttpClient = new HttpClientConfig(section);

            config.Yakeen = new YakeenConfig(section);
            config.Najm = new NajmConfig(section);
            config.NajmNewService = new NajmNewServiceConfig(section);

            config.SaudiPost = new SaudiPostConfig(section);
            config.Inquiry = new InquiryConfig(section);
            config.Settings = new SettingsConfig(section);
            config.Identity = new IdentityConfig(section);
            config.GoogleCaptchaConfig = new GoogleCaptchaConfig(section);
            config.AdministrationConfig = new AdministrationConfig(section);
            config.Sadad = new SadadConfig(section);
            config.RiyadBank = new RiyadBankConfig(section);
            config.Hyperpay = new HyperpayConfig(section);
            config.BcareInsuranceCompanyConfig = new BcareInsuranceCompanyConfig(section);
            config.ClientUrl = new ClientUrlConfig(section);
            config.RemoteServerInfo = new RemoteServerInfo(section);
            config.Edaat = new EdaatConfig(section);
            config.Tabby = new TabbyConfig(section);
            config.WathqConfig = new WathqConfig(section);
            return config;
        }



        #region Properties



        /// <summary>
        /// A value indicating whether the site is run on multiple instances (e.g. web farm, Windows Azure with multiple instances, etc).
        /// Do not enable it if you run on Azure but use one instance only
        /// </summary>
        public bool MultipleInstancesEnabled { get; private set; }



        /// <summary>
        /// A value indicating whether the site is run on Windows Azure Websites
        /// </summary>
        public bool RunOnAzureWebsites { get; private set; }


        /// <summary>
        /// Indicates whether we should ignore startup tasks
        /// </summary>
        public bool IgnoreStartupTasks { get; private set; }
        public SMTPConfig SMTP { get; set; }




        public PolicyConfig Policy { get; private set; }
        public PayFortConfig PayFort { get; private set; }
        public QuotatoinConfig Quotatoin { get; private set; }
        public HttpClientConfig HttpClient { get; private set; }

        /// <summary>
        /// Yakeen configuration.
        /// </summary>
        public YakeenConfig Yakeen { get; private set; }

        /// <summary>
        /// Najm configuration.
        /// </summary>
        public NajmConfig Najm { get; private set; }

        public NajmNewServiceConfig NajmNewService { get; private set; }

        /// <summary>
        /// Saudi post configuration.
        /// </summary>
        public SaudiPostConfig SaudiPost { get; private set; }

        /// <summary>
        /// Inquiry service configuration
        /// </summary>
        public InquiryConfig Inquiry { get; private set; }
        public SettingsConfig Settings { get; private set; }
        public IdentityConfig Identity { get; private set; }
        public GoogleCaptchaConfig GoogleCaptchaConfig { get; private set; }
        public AdministrationConfig AdministrationConfig { get; private set; }
        public SadadConfig Sadad { get; private set; }

        public EdaatConfig Edaat { get; private set; }

        /// <summary>
        /// RiydBank MIGS payment configuration.
        /// </summary>
        public RiyadBankConfig RiyadBank { get; private set; }
        public HyperpayConfig Hyperpay { get; private set; }
        public ClientUrlConfig ClientUrl { get; private set; }

        public BcareInsuranceCompanyConfig BcareInsuranceCompanyConfig { get; set; }
        public RemoteServerInfo RemoteServerInfo { get; set; }
        public TabbyConfig Tabby { get; private set; }
        public WathqConfig WathqConfig { get; private set; }

        #endregion

        #region Private Methods




        private SMTPConfig CreateSMTPObj(XmlNode section)
        {
            var SMTP = new SMTPConfig();
            var smtpNode = section.SelectSingleNode("SMTP");
            SMTP.Host = smtpNode.GetString("Host");
            SMTP.EnableSsl = smtpNode.GetBool("EnableSsl");
            SMTP.Port = smtpNode.GetInteger("Port");
            SMTP.Timeout = smtpNode.GetInteger("Timeout");
            SMTP.UseDefaultCredentials = smtpNode.GetBool("UseDefaultCredentials");
            SMTP.SenderEmailAddress = smtpNode.GetString("SenderEmailAddress");
            SMTP.Credentials = new NetworkCredential(SMTP.SenderEmailAddress, "Ohgd@Aa@care!@!!4");
            SMTP.DeliveryMethod = SmtpDeliveryMethod.Network;
            return SMTP;

        }

        #endregion
    }
}