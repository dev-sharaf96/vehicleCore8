namespace Tamkeen.bll.UnitOfWork
{
    public class LookupUOW : ILookupUOW
    {
        private LookupUOW() { }
        private static LookupUOW lookupUOW = null;

        public static LookupUOW Instance
        {
            get
            {
                if (lookupUOW == null)
                {
                    lookupUOW = new LookupUOW();
                }

                return lookupUOW;
            }
        }

        public Lookups.IYearLookup YearLookup { get => Lookups.YearLookup.Instance; }

        public Lookups.ICityLookup CityLookup { get => Lookups.CityLookup.Instance; }

        public Lookups.IBankLookup BankLookup { get => Lookups.BankLookup.Instance; }
        
        public Lookups.IColorLookup ColorLookup { get => Lookups.ColorLookup.Instance; }
        
        public Lookups.IBreakingSystemLookup BreakingSystemLookup { get => Lookups.BreakingSystemLookup.Instance; }

        public Lookups.ISpeedStabilizerLookup SpeedStabilizerLookup { get => Lookups.SpeedStabilizerLookup.Instance; }
        public Lookups.ISensorLookup SensorLookup { get => Lookups.SensorLookup.Instance; }
        public Lookups.IDistanceRangeLookup DistanceRangeLookup { get => Lookups.DistanceRangeLookup.Instance; }
        public Lookups.ICameraTypeLookup CameraTypeLookup { get => Lookups.CameraTypeLookup.Instance; }
        public Lookups.IParkingPlaceLookup ParkingPlaceLookup { get => Lookups.ParkingPlaceLookup.Instance; }
        public Lookups.IVehicleTransmissionTypeLookup VehicleTransmissionTypeLookup { get => Lookups.VehicleTransmissionTypeLookup.Instance; }
        public Lookups.ICountryLookup CountryLookup { get => Lookups.CountryLookup.Instance; }
        public Lookups.IDrivingLicenceYearLookup DrivingLicenceYearLookup { get => Lookups.DrivingLicenceYearLookup.Instance; }
        public Lookups.IDriverMedicalConditionLookup DriverMedicalConditionLookup { get => Lookups.DriverMedicalConditionLookup.Instance; }
        public Lookups.IVehicleUsagePercentageLookup VehicleUsagePercentageLookup{ get => Lookups.VehicleUsagePercentageLookup.Instance; }
        
    }
}
