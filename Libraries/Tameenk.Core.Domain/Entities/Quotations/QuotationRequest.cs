using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Core.Domain.Entities.Quotations
{
    public class QuotationRequest : BaseEntity
    {
        public QuotationRequest()
        {
            QuotationResponses = new HashSet<QuotationResponse>();
            Drivers = new HashSet<Driver>();
        }

        public int ID { get; set; }

        public string ExternalId { get; set; }

        public Guid MainDriverId { get; set; }

        public long CityCode { get; set; }

        public DateTime? RequestPolicyEffectiveDate { get; set; }

        public Guid VehicleId { get; set; }

        public string UserId { get; set; }

        public string NajmNcdRefrence { get; set; }

        public int? NajmNcdFreeYears { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public bool IsComprehensiveGenerated { get; set; }

        public bool IsComprehensiveRequested { get; set; }

        /// <summary>
        /// Insured person information identifier.
        /// </summary>
        public int InsuredId { get; set; }
        public int? NoOfAccident { get; set; }
        public string NajmResponse { get; set; }
        public bool? ManualEntry { get; set; }
        public bool? IsRenewal { get; set; }
        public string PostCode { get; set; }
        public string PreviousReferenceId { get; set; }
        public string MissingFields { get; set; }
        public Guid? AdditionalDriverIdOne { get; set; }
        public Guid? AdditionalDriverIdTwo { get; set; }
        public Guid? AdditionalDriverIdThree { get; set; }
        public Guid? AdditionalDriverIdFour { get; set; }
        public Guid? AutoleasingTransactionId { get; set; }
        public bool? AutoleasingBulkOption { get; set; }
        public bool? AutoleasingInitialOption { get; set; }
        public int? AutoleasingContractDuration { get; set; }
        public bool? IsConverted { get; set; }
        public bool? ShowInitial { get; set; }
        public string InitialExternalId { get; set; }

        public AspNetUser AspNetUser { get; set; }

        public City City { get; set; }

        public Driver Driver { get; set; }

        public Vehicle Vehicle { get; set; }
        /// <summary>
        /// Insured information.
        /// </summary>
        public Insured Insured { get; set; }

        public ICollection<QuotationResponse> QuotationResponses { get; set; }

        public ICollection<Driver> Drivers { get; set; }
    }
}
