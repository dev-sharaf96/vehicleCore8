﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class To
    {
        [JsonProperty("number")]
        public string Number { get; set; }
    }
}
