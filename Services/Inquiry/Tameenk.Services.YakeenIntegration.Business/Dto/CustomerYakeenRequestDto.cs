﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.YakeenIntegration.Business.Dto
{
    public class CustomerYakeenRequestDto
    {
        public readonly string ReferenceNumber;
        public long Nin { get; set; }
        public bool IsCitizen { get; set; }
        public string DateOfBirth { get; set; }

        public CustomerYakeenRequestDto()
        {
            ReferenceNumber = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 15);
        }
    }
}