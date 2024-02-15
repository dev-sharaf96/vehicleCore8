using System;
using System.Collections.Generic;
using AutoMapper;
using Tameenk.Services.Core.Leasing.Models;

namespace Tameenk.IVR.TicketApi.Infrastructure
{
    /// <summary>
    /// Represent the AutoMapper configuration.
    /// For more info look at https://automapper.org/
    /// </summary>
    public static class AutoMapperConfiguration
    {
        /// <summary>
        /// Initialize the configuration of auto mapper.
        /// </summary>
        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DriverInsuranceRecordFromDB, DriverData>();
                cfg.CreateMap<DriverData, DriverInsuranceRecordFromDB>();

                cfg.CreateMap<DriverInsuranceRecordFromDB, VehicleData>();
                cfg.CreateMap<VehicleData, DriverInsuranceRecordFromDB>();

                cfg.CreateMap<DriverInsuranceRecordFromDB, ContractData>();
                cfg.CreateMap<ContractData, DriverInsuranceRecordFromDB>();
            });
            Mapper = MapperConfiguration.CreateMapper();
        }


        /// <summary>
        /// Mapper
        /// </summary>
        public static IMapper Mapper { get; private set; }

        /// <summary>
        /// Mapper configuration
        /// </summary>
        public static MapperConfiguration MapperConfiguration { get; private set; }

    }
}