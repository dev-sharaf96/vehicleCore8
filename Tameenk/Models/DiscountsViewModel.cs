﻿using System;
    public class DiscountsViewModel
    {
        public string DriverNIN { get; set; }
        public CaptchaModel Captcha { get; set; }
        public string Language { get; set; }
        public string ErrorMessage { get; set; }
        public ActivePolicyData ActivePolicyData { get; set; }
    }