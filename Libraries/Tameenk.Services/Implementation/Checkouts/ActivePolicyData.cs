﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class ActivePolicyData
    {
        public int HasActivePolicy { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}