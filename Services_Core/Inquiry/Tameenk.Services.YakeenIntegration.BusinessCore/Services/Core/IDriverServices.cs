﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Integration.Dto.Yakeen;

namespace Tameenk.Services.YakeenIntegration.Business.Services.Core
{
    public interface IDriverServices
    {
        //DriverYakeenInfoModel GetDriverByOfficialIdAndLicenseExpiryDate(DriverYakeenInfoRequestModel driverInfoRequest);
        DriverYakeenInfoModel GetDriverByTameenkId(Guid TameenkId);
    }
}