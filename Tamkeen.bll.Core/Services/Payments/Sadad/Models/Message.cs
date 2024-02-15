using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Tamkeen.bll.Services.Sadad.Models
{
    [DataContract(Name = "Message", Namespace = "")]
    public class Message
    {
        [DataMember(Order = 1)]
        public virtual Header Header { get; set; }
        [DataMember(Order = 7)]
        public virtual Body Body { get; set; }
    }
    [DataContract(Name = "Header", Namespace = "")]
    public class Header
    {
        //public int ID { get; set; }
        [DataMember(Order = 2)]
        public string Receiver { get; set; }
        [DataMember(Order = 3)]
        public string Sender { get; set; }
        [DataMember(Order = 4)]
        public string MessageType { get; set; }
        [DataMember(Order = 5)]
        public DateTime TimeStamp { get; set; }
    }
    [DataContract(Name = "Body", Namespace = "")]
    public class Body
    {
        [DataMember(Order = 8)]
        public string AccountNo { get; set; }
        [DataMember(Order = 9)]
        public Double Amount { get; set; }
        [DataMember(Order = 10)]
        public string CustomerRefNo { get; set; }
        [DataMember(Order = 11)]
        public string TransType { get; set; }
        [DataMember(Order = 12)]
        public string Description { get; set; }
    }


    [DataContract(Name = "Message", Namespace = "")]
    public class MessageResponse
    {
        [DataMember(Order = 1)]
        public virtual Header Header { get; set; }
        [DataMember(Order = 2)]
        public string Status { get; set; }
    }

}