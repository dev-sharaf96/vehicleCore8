using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tamkeen.bll.Model
{
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
    }
}