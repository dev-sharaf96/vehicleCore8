using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tamkeen.bll.Services.Sadad.Models
{

    public class MessageWebSrevice
    {
        [Key]
        public int ID { get; set; }
        public string HeadersReceiver { get; set; }
        public string HeadersSender { get; set; }
        public string HeadersMessageType { get; set; }
        public DateTime HeadersTimeStamp { get; set; }
        public string BodysAccountNo { get; set; }
        public Double BodysAmount { get; set; }
        public string BodysCustomerRefNo { get; set; }
        public string BodysTransType { get; set; }
        public string BodysDescription { get; set; }
        public string MessageResponsesStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}