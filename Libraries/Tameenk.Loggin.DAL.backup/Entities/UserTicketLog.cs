﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    [Table("UserTicketLog")]

    public class UserTicketLog
    {
        public int ID { get; set; }
        public string MethodName { get; set; }
        public string UserId { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        public string ServerIP { get; set; }
        public string Channel { get; set; }
        public string DriverNin { get; set; }
        public string ReferenceId { get; set; }

        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
