﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.AdministrationApi.AutoleasingOutput
{
    public class LookupsOutput
    {

        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            InvalidCaptcha=4,
            ServiceException = 5,
            OwnerNationalIdAndNationalIdAreEqual=6
        }

        public string ErrorDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }

        public List<Tameenk.Core.Domain.Dtos.Lookup> Result { get; set; }

    }
}
