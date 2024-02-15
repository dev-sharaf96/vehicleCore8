
using System.Xml.Serialization;

namespace Tameenk.Core.Domain.Entities.Payments.Sadad
{
  
    [XmlRoot(ElementName = "sadadbilluploadstatus")]
    public class SadadResponse : BaseEntity
    {
        public int Id { get; set; }

        public int SadadRequestId { get; set; }

        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("errorCode")]
        public int ErrorCode { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("trackingId")]
        public int TrackingId { get; set; }

        public SadadRequest SadadRequest { get; set; }
    }
}
