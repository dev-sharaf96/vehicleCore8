﻿using System;
        public string UserId { get; set; }
        public bool ActiveTabbyTPL { get; set; }
        public bool ActiveTabbyWafiSmart { get; set; }



        #region Product
        public int? ProductInsuranceTypeCode { get; set; }
        public int? VehicleLimitValue { get; set; }

        #endregion
        #region InsuranceCompany
        public bool UsePhoneCamera { get; set; }




        #endregion
        #region QuotationResponse


        #endregion
    }



        #region Product_Benefit




        #endregion
        #region Benefit


        #endregion



        #region PriceType


        #endregion
        public Guid DriverId { get; set; }
        public decimal DriverPrice { get; set; }
        public string DriverExternalId { get; set; }