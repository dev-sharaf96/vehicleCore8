using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Tamkeen.bll.Services.Sadad.Models
{
    [XmlRoot(ElementName = "sadadbilluploadstatus")]
    public class SadadResponse
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public string description { get; set; }
        public int trackingId { get; set; }
    }
}