﻿using System;
using System.Collections.Generic;

namespace Tameenk.Services.Implementation
{
    public class CorporateModel
    {
       public List<InsuredPolicyInfo>? InsuredPolicies { get; set; }
       public EdaatRequestModel? EdaatRequest { get; set; }
    }

    public class EdaatRequestModel
    {
        public DateTime ? ExpiryDate { get; set; }
        public  string? InsuredNationalId { get; set; }
        public  string? NationalID { get; set; }
        public int EdaatResponseId { get; set; }
        public  string? EdaatResponseCode { get; set; }
        public bool IsEdaatResponseSuccess { get; set; }
    }
}
