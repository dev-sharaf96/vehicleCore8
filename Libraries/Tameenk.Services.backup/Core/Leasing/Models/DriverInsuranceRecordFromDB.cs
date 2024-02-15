using System;

namespace Tameenk.Services.Core.Leasing.Models
{
    public class DriverInsuranceRecordFromDB
    {
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string NIN { get; set; }
        public string MobileNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }

        public short? ModelYear { get; set; }
        public string VehicleModel { get; set; }
        public int? VehicleValue { get; set; }

        public DateTime? StartDate { get; set; } //PolicyIssueDate
        public int? Remains { get; set; }
        public int? Duration { get; set; }
        public string CustomCardNumber { get; set; }
        public string SequenceNumber { get; set; }
        public string ExternalId { get; set; }
        public int ? BankId { get; set; }
    }

    public class DriverData
    {
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string NIN { get; set; }
        public string MobileNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
    }

    public class VehicleData
    {
        public short? ModelYear { get; set; }
        public string VehicleModel { get; set; }
        public int? VehicleValue { get; set; }
    }

    public class ContractData
    {
        public DateTime? StartDate { get; set; } //PolicyIssueDate
        public int? Remains { get; set; }
        public int? Duration { get; set; }
    }

    public class DriverInsuranceRecord
    {
        public DriverData driverData { get; set; }
        public VehicleData vehicleData { get; set; }
        public ContractData contractData { get; set; }
        public CustomerBalanceDetails balanceData { get; set; }

    }
    public class CustomerBalanceDetails
    {
        public decimal? ChargedData { get; set; }
        public decimal ?PayedAmount { get; set; }
        public decimal? BalancAmount { get; set; }
    }
}