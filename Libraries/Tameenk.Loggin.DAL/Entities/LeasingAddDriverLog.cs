﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("LeasingAddDriverLog")]
    public class LeasingAddDriverLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string PageURL { get; set; }
        public string PageName { get; set; }
        public int? BankID { get; set; }
        public string BankName { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string ServiceRequest { get; set; }
        public string ServiceResponse { get; set; }
        public string ServerIP { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public string MethodName { get; set; }
        public string ApiURL { get; set; }
    }
}