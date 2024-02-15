using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Payments.Tabby
{
    public class TabbyResponseHandler
    {
        public TabbyResponseStatus Status { set; get; } = new TabbyResponseStatus();
        public TabbyResponseModel ResponseBody { set; get; } = new TabbyResponseModel();
        public string TabbyUrl { set; get; } 
    }
    public class TabbyResponseStatus
    {
        public string status { set; get; }
       public string errorType { get; set; }
        public string error { get; set; }
        public List<TabbyErrorStatus> errors { get; set; }
        public bool IsErrors { get; set; } = false;
    }

    public class TabbyErrorStatus
    {
        public string field { set; get; }
        public string code { get; set; }
        public string message { get; set; }
    }
}
