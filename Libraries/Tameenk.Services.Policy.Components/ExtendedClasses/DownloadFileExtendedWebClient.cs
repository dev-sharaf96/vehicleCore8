using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Common.Utilities;
using System.Net;

namespace Tameenk.Services.Policy.Components
{
    public class DownloadFileExtendedWebClient : WebClient
    {
        //time in milliseconds
        private int timeout;
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public DownloadFileExtendedWebClient()
        {
            this.timeout = 60000;
        }

        public DownloadFileExtendedWebClient(int timeout)
        {
            this.timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var result = base.GetWebRequest(address);
            result.Timeout = this.timeout;
            return result;
        }
    }
}
