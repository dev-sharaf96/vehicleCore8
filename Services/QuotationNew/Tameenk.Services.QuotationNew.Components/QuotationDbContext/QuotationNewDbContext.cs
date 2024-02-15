
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Services.QuotationNew.Components.QuotationDbContext
{
    public class QuotationNewDbContext : Microsoft.EntityFrameworkCore.DbContext
    {

        public QuotationNewDbContext(DbContextOptions<QuotationNewDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlServer("your_connection_string_here");
        }

        public QuotationNewRequestDetails GetFromCheckoutByReferenceId(string externalId)
        {
            QuotationNewRequestDetails request = new QuotationNewRequestDetails();
            using (var connection = Database.GetDbConnection() as SqlConnection)
            {
                if (connection == null)
                    throw new InvalidOperationException("The connection is not a valid SQL connection.");

                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                using (var command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = "GetQuotationRequestDetailsByExternalId";
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter nationalIDParameter = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                        command.Parameters.Add(nationalIDParameter);
                        var reader = command.ExecuteReader();


                        request.AdditionalDrivers = new List<Driver>();
                        request.MainDriverViolation = new List<DriverViolation>();
                        request.MainDriverLicenses = new List<DriverLicense>();

                        request.ID = Convert.ToInt32(reader["ID"]);
                        request.ExternalId = Convert.ToString(reader["ExternalId"]);
                        request.RequestPolicyEffectiveDate = Convert.ToDateTime(reader["RequestPolicyEffectiveDate"]);
                        request.NajmNcdFreeYears = Convert.ToInt32(reader["NajmNcdFreeYears"]);
                        request.NajmNcdRefrence = Convert.ToString( reader["NajmNcdRefrence"]);
                        request.PostCode = Convert.ToString( reader["PostCode"]);
                        request.IsRenewal = Convert.ToBoolean(reader["IsRenewal"]);
                        request.NoOfAccident = Convert.ToInt32(reader["NoOfAcciden"]);
                        request.NajmResponse = Convert.ToString(reader["NajmResponse"]);
                        request.MainDriverId = Guid.Parse(reader["MainDriverId"].ToString());
                        request.VehicleId = Guid.Parse(reader["VehicleId"].ToString());
                        request.CityCode = Convert.ToInt64(reader["CityCode"]);
                        request.MissingFields = Convert.ToString(reader["MissingFields"]);
                        request.InsuredId = Convert.ToInt32(reader["InsuredId"]);
                        request.QuotationCreatedDate = Convert.ToDateTime( reader["QuotationCreatedDate"]);
                        request.NationalId = Convert.ToString(reader["NationalId"]);
                        request.InsuredBirthDate = Convert.ToDateTime(reader["InsuredBirthDate"]);
                        request.InsuredBirthDateH = Convert.ToString(reader["InsuredBirthDateH"]);
                        request.InsuredGenderId = Convert.ToInt32(reader["InsuredGenderId"]);
                        request.NationalityCode = Convert.ToString(reader["NationalityCode"]);
                        request.InsuredFirstNameAr = Convert.ToString(reader["InsuredFirstNameAr"]);
                        request.ManualEntry = Convert.ToBoolean(reader["ManualEntry"]);
                        request.HasAntiTheftAlarm = Convert.ToBoolean(reader["HasAntiTheftAlarm"]);
                        request.HasFireExtinguisher = Convert.ToBoolean(reader["HasFireExtinguisher"]);
                        request.HasTrailer = Convert.ToBoolean(reader["HasTrailer"]);
                        request.TrailerSumInsured = Convert.ToInt32(reader["TrailerSumInsured"]);
                        request.OtherUses = Convert.ToBoolean( reader["OtherUses"]);
                        request.IsRenewal = Convert.ToBoolean(reader["IsRenewal"]);
                        // request.VehicleUseId = Convert.ToInt32(reader["UserId"]);

                        if (request.ID > 0)
                        {
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    Driver driver = new Driver();
                                    driver.DriverId = Guid.Parse(reader["DriverId"].ToString());
                                    driver.IsCitizen = Convert.ToBoolean(reader["IsCitizen"]);
                                    driver.NIN = Convert.ToString(reader["NIN"]);
                                    request.AdditionalDrivers.Add(driver);
                                }
                            }

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    DriverViolation driverViolation = new DriverViolation();
                                    driverViolation.Id = Convert.ToInt32(reader["Id"]);
                                    driverViolation.DriverId = Guid.Parse(reader["DriverId"].ToString());
                                    driverViolation.ViolationId = Convert.ToInt32(reader["ViolationId"]);
                                    driverViolation.InsuredId = Convert.ToInt32(reader["InsuredId"]);
                                    driverViolation.NIN = Convert.ToString(reader["NIN"]);
                                    request.MainDriverViolation.Add(driverViolation);
                                }
                            }

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    DriverLicense driverLicense = new DriverLicense();
                                    driverLicense.LicenseId = Convert.ToInt32(reader["LicenseId"]);
                                    driverLicense.DriverId = Guid.Parse(reader["DriverId"].ToString());
                                    driverLicense.TypeDesc = Convert.ToInt16(reader["TypeDesc"]);
                                    driverLicense.ExpiryDateH = Convert.ToString(reader["ExpiryDateH"]);
                                    driverLicense.IssueDateH = Convert.ToString(reader["IssueDateH"]);
                                    driverLicense.licnsTypeDesc = reader["licnsTypeDesc"].ToString();
                                    request.MainDriverLicenses.Add(driverLicense);
                                }
                            }
                        }

                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                    }
                }
            }
            return request;
        }

        private Address GetAddressesByNin(string driverNin)
        {
            Address address = null;
            using (var connection = Database.GetDbConnection() as SqlConnection)
            {
                if (connection == null)
                    throw new InvalidOperationException("The connection is not a valid SQL connection.");

                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                using var command = connection.CreateCommand();
                try
                {
                    command.CommandText = "GetAddress";
                    command.CommandType = CommandType.StoredProcedure;
                    SqlParameter nationalIDParameter = new SqlParameter() { ParameterName = "@driverNin", Value = driverNin };
                    command.Parameters.Add(nationalIDParameter);
                    var reader = command.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        address = new Address();
                        address.Id = Convert.ToInt32(reader["Id"]);
                        address.Title = Convert.ToString(reader["Title"]);
                        address.Address1 = Convert.ToString(reader["Address1"]);
                        address.Address2 = Convert.ToString(reader["Address2"]);
                        address.ObjLatLng = Convert.ToString(reader["ObjLatLng"]);
                        address.BuildingNumber = Convert.ToString(reader["BuildingNumber"]);
                        address.Street = Convert.ToString(reader["Street"]);
                        address.District = Convert.ToString(reader["District"]);
                        address.City = Convert.ToString(reader["City"]);
                        address.PostCode = Convert.ToString(reader["PostCode"]);
                        address.AdditionalNumber = Convert.ToString(reader["AdditionalNumber"]);
                        address.RegionName = Convert.ToString(reader["RegionName"]);
                        address.PolygonString = Convert.ToString(reader["PolygonString"]);
                        address.IsPrimaryAddress = Convert.ToString(reader["IsPrimaryAddress"]);
                        address.UnitNumber = Convert.ToString(reader["UnitNumber"]);
                        address.Latitude = Convert.ToString(reader["Latitude"]);
                        address.Longitude = Convert.ToString(reader["Longitude"]);
                        address.CityId = Convert.ToString(reader["CityId"]);
                        address.RegionId = Convert.ToString(reader["RegionId"]);
                        address.Restriction = Convert.ToString(reader["Restriction"]);
                        address.PKAddressID = Convert.ToString(reader["PKAddressID"]);
                        address.DriverId = Guid.Parse(reader["DriverId"].ToString());
                        address.AddressLoction = Convert.ToString(reader["AddressLoction"]);
                        address.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
                        address.NationalId = Convert.ToString(reader["NationalId"]);
                        address.IsDeleted = Convert.ToBoolean(reader["IsDeleted"]);
                        address.ModifiedDate = Convert.ToDateTime(reader["ModifiedDate"]);
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    connection.Close();
                }
            }
            return address;
        }

        public PromotionProgramUserModel GetUserPromotionCodeInfo(string userId, string nationalId, int insuranceCompanyId, int insuranceTypeCode)
        {
            PromotionProgramUserModel promotionProgramUserModel = null;
            Address address = new Address();
            using (var connection = Database.GetDbConnection() as SqlConnection)
            {
                if (connection == null)
                    throw new InvalidOperationException("The connection is not a valid SQL connection.");

                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                using var command = connection.CreateCommand();
                try
                {
                    command.CommandText = "GetUserPromotionProgramInfo";
                    command.CommandType = CommandType.StoredProcedure;
                    if (!string.IsNullOrEmpty(userId) && userId != Guid.Empty.ToString())
                    {
                        SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
                        command.Parameters.Add(userIdParam);
                    }
                    SqlParameter nationalIdParam = new SqlParameter() { ParameterName = "nationalId", Value = nationalId };
                    command.Parameters.Add(nationalIdParam);

                    SqlParameter insuranceCompanyIdParam = new SqlParameter() { ParameterName = "insuranceCompanyId", Value = insuranceCompanyId };
                    SqlParameter insuranceTypeCodeParam = new SqlParameter() { ParameterName = "insuranceTypeCode", Value = insuranceTypeCode };

                    command.Parameters.Add(insuranceCompanyIdParam);
                    command.Parameters.Add(insuranceTypeCodeParam);
                    var reader = command.ExecuteReader();

                    if (reader!=null && reader.HasRows)
                    {
                        promotionProgramUserModel.NationalId = Convert.ToString(reader["NationalId"]);
                        promotionProgramUserModel.Email = Convert.ToString(reader["Email"]);
                        promotionProgramUserModel.Code = Convert.ToString(reader["Code"]);
                        promotionProgramUserModel.PromotionProgramId = Convert.ToInt32(reader["PromotionProgramId"]);

                        if (String.IsNullOrWhiteSpace(promotionProgramUserModel.NationalId) || promotionProgramUserModel.NationalId == null)
                        {
                            reader.NextResult();
                            promotionProgramUserModel.Email = Convert.ToString(reader["Email"]);
                            promotionProgramUserModel.Code = Convert.ToString(reader["Code"]);
                            promotionProgramUserModel.PromotionProgramId = Convert.ToInt32(reader["PromotionProgramId"]);
                        }
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    connection.Close();
                    return null;
                }
            }
            return promotionProgramUserModel;
        }
    }
}