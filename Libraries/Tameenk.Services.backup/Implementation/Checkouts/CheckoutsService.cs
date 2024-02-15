﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Caching;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation.Orders;

namespace Tameenk.Services.Implementation.Checkouts
{
    public class CheckoutsService : ICheckoutsService
    {


        #region Fields

        private readonly IRepository<CheckoutDetail> _checkOutDetailsRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<CheckoutDriverInfo> _checkoutDriverInfoRepository;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<CheckoutCarImage> _carImageRepository;
        #endregion

        #region Ctor

        public CheckoutsService(
            ICacheManager cacheManager,
            IRepository<CheckoutDetail> checkOutDetailsRepository
            , IRepository<CheckoutDriverInfo> checkoutDriverInfoRepository,
            IRepository<Driver> driverRepository,
            IRepository<CheckoutCarImage> carImageRepository)
        {
            _cacheManager = cacheManager ?? throw new TameenkArgumentNullException(nameof(ICacheManager));
            _checkOutDetailsRepository = checkOutDetailsRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<CheckoutDetail>));
            _checkoutDriverInfoRepository = checkoutDriverInfoRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<CheckoutDriverInfo>));
            _carImageRepository = carImageRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<CheckoutCarImage>));

        }

        #endregion

        #region Methods
        /// <summary>
        /// Get all Checkouts based on filter
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        public IPagedList<CheckoutDetail> GetCheckoutsWithFilter(IQueryable<CheckoutDetail> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "ReferenceId", bool sortOrder = false)
        {
            return new PagedList<CheckoutDetail>(query, pageIndex, pageSize, sortField, sortOrder);
        }

        /// <summary>
        /// Prepare query for Checkout Details With Filter
        /// </summary>
        /// <param name="checkoutsFilter">checkouts Filter</param>
        /// <returns></returns>
        public IQueryable<CheckoutDetail> PrepareCheckoutDetailsQueryWithFilter(CheckoutsFilter checkoutsFilter)
        {
            var query = _checkOutDetailsRepository.Table
                .Include(x=>x.Driver)
                .Include(x=>x.Vehicle)
                .Include(x => x.PolicyStatus)
                .Include(x => x.PaymentMethod);

            if (!string.IsNullOrEmpty(checkoutsFilter.SequenceNumber))
                query = query.Where(p => p.Vehicle.SequenceNumber == checkoutsFilter.SequenceNumber);

            if (!string.IsNullOrEmpty(checkoutsFilter.ReferenceId))
                query = query.Where(p => p.ReferenceId == checkoutsFilter.ReferenceId);

            if (!string.IsNullOrEmpty(checkoutsFilter.NIN))
                query = query.Where(p => p.Driver.NIN == checkoutsFilter.NIN);

            if (!string.IsNullOrEmpty(checkoutsFilter.MerchantId))
            {
                Guid? _MerchantId = Guid.Parse(checkoutsFilter.MerchantId);
                query = query.Where(p => p.MerchantTransactionId == _MerchantId);
            }
            return query;
        }

        public CheckoutDetail GetCheckoutDetailsByReferenceId(string referenceId)
        {
            CheckoutDetail checkoutDetail = _checkOutDetailsRepository.Table
                .Include(x => x.Driver)
                .Include(x => x.Vehicle)
                .Include(x => x.PolicyStatus)
                .Include(x => x.PaymentMethod)
                .Include(e => e.ImageRight)
                 .Include(e => e.ImageLeft)
                 .Include(e => e.ImageFront)
                 .Include(e => e.ImageBack)
                 .Include(e => e.ImageBody)
                 .FirstOrDefault(c=>c.ReferenceId == referenceId);

            return checkoutDetail;
        }

        public CheckoutDriverInfo GetAllCheckedoutPoliciesBasedOnFilter(PolicyCheckoutFilter policyFilter)
            {
            {
                {
                    {
        public CheckoutDriverInfo GetCheckoutDriverInfo(string nin, string phone, string email, string iban)
            {
                phoneWithout966 = phoneformatted.Substring("966".Length);
                phoneWithout966 = "0" + phoneWithout966;
            }
                _checkoutDriverInfoRepository.TableNoTracking

        /// <summary>
        /// Get All Channel
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Channels> GetAllChannel()
        {
            var channelList = new List<Channels>();
            var values = Enum.GetValues(typeof(Tameenk.Common.Utilities.Channel));

            foreach (Tameenk.Common.Utilities.Channel val in values)
            {
                channelList.Add(new Channels() { ChannelCode = val, Name = Enum.GetName(typeof(Tameenk.Common.Utilities.Channel), val) });
            }

            return channelList;
        }

        public List<UserPolicyModel> GetUserPolicies(string nin, string sequenceNumber, string customCardNumber, out string exception)

        public List<FailedPolicyModel> GetUserFailedPolicies(string nin, string sequenceNumber, string customCardNumber, string referenceId, out string exception)
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@Nin", Value = nin });
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@SequenceNumber", Value = sequenceNumber });
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@CustomCardNumber", Value = customCardNumber });
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@referenceId", Value = referenceId });

        public int GetUserSuccessPolicies(string driverNin, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetUserSuccessPolicies";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "@DriverNin", Value = driverNin };
                command.Parameters.Add(userIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int list = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return list;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return 0;
            }
        }

        public CheckoutDetail GetCheckoutDetailByReferenceIdAndUserId(string referenceId, string userId)
        {
            var checkoutDetail = _checkOutDetailsRepository.TableNoTracking
                .Where(c => c.IsCancelled == false && c.ReferenceId == referenceId && c.UserId == userId)
                .FirstOrDefault();

            return checkoutDetail;
        }

        public int GetVerifiedEmailCheckoutDetail(string driverNin, string email, out string exception)
        {
            exception = string.Empty;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetVerifiedEmailCheckoutDetail";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter driverNinParam = new SqlParameter() { ParameterName = "@driverNin", Value = driverNin };
                SqlParameter emailParam = new SqlParameter() { ParameterName = "@email", Value = email };
                command.Parameters.Add(driverNinParam);
                command.Parameters.Add(emailParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int count = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return count;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return 0;
            }
        }

        public int GetTotalVehicleSadadRequestsPerDay(string vehicleId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetTotalVehicleSadadRequestsPerDay";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter vehicleIdParam = new SqlParameter() { ParameterName = "@VehicleId", Value = vehicleId };
                command.Parameters.Add(vehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int list = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return list;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return 0;
            }
        }

        public List<CheckoutDetail> GetAdditionalDriversToUpdateCheckOutTemp(out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAdditionalDrivers";
                command.CommandType = CommandType.StoredProcedure;
                //SqlParameter vehicleIdParam = new SqlParameter() { ParameterName = "@VehicleId", Value = vehicleId };
                //command.Parameters.Add(vehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<CheckoutDetail> list = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetail>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return list;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public List<CheckoutDetail> GetAdditionalDriversToUpdateCheckOutTemp2(out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAdditionalDrivers2";
                command.CommandType = CommandType.StoredProcedure;
                //SqlParameter vehicleIdParam = new SqlParameter() { ParameterName = "@VehicleId", Value = vehicleId };
                //command.Parameters.Add(vehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<CheckoutDetail> list = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetail>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return list;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public List<CheckoutDetail> GetAdditionalDriversToUpdateCheckOutTemp3(out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAdditionalDrivers3";
                command.CommandType = CommandType.StoredProcedure;
                //SqlParameter vehicleIdParam = new SqlParameter() { ParameterName = "@VehicleId", Value = vehicleId };
                //command.Parameters.Add(vehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<CheckoutDetail> list = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetail>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return list;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public List<CheckoutDetail> GetAdditionalDriversToUpdateCheckOutTemp4(out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAdditionalDrivers4";
                command.CommandType = CommandType.StoredProcedure;
                //SqlParameter vehicleIdParam = new SqlParameter() { ParameterName = "@VehicleId", Value = vehicleId };
                //command.Parameters.Add(vehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<CheckoutDetail> list = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetail>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return list;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public CheckoutDetail GetComprehansiveCheckoutDetail(string referenceId)
        {
            var checkoutDetail = _checkOutDetailsRepository.Table
                .Where(c => c.IsCancelled == false && c.ReferenceId == referenceId && c.SelectedInsuranceTypeCode == 2)
                .FirstOrDefault();

            return checkoutDetail;
        }

        public CheckoutCarImage AddChekoutCarImages(CheckoutCarImage carImage, out string exception)
                return carImage;


        public bool UpdateCheckOut(CheckoutDetail details, out string exception)
                return true;
        public bool UpdateCheckOutDetails(string referenceId)
            var item = _checkOutDetailsRepository.Table.Where(a => a.ReferenceId == referenceId).FirstOrDefault();
            if (item != null)
            {
                item.ModifiedDate = DateTime.Now;
                _checkOutDetailsRepository.Update(item);
                return true;
            }
            return false;
        }
        public List<RenewalPolicyInfo> GetRenewalPolicies(DateTime start, DateTime end,int notificationNo, out string exception)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                exception = string.Empty;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetRenewalPolicies";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;
                SqlParameter startDateParam = new SqlParameter() { ParameterName = "startDate", Value = start };
                SqlParameter endDateParam = new SqlParameter() { ParameterName = "endDate", Value = end };
                SqlParameter notificationNoParam = new SqlParameter() { ParameterName = "notificationNo", Value = notificationNo };

                command.Parameters.Add(startDateParam);
                command.Parameters.Add(endDateParam);
                command.Parameters.Add(notificationNoParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var info = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RenewalPolicyInfo>(reader).ToList();
                if (info != null)
                {
                    return info;
                }
                return info;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        public CheckoutDetail GetFromCheckoutDeatilsbyReferenceId(string referenceId)
        {
            return _checkOutDetailsRepository.TableNoTracking.Where(c => c.ReferenceId == referenceId).FirstOrDefault();
        }

        public ActivePolicyData UserHasActivePolicy(string driverNin, out string exception)

                idbContext.DatabaseInstance.Connection.Close();

        public List<PolicyInformation> GetPolicyInformationForRoadAssistance(out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyInformationForRoadAssistance";
                command.CommandType = CommandType.StoredProcedure;
                DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-2);
                DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(-1);
                command.CommandTimeout = 600;
                SqlParameter startDateParam = new SqlParameter() { ParameterName = "startDate", Value = start };
                SqlParameter endDateParam = new SqlParameter() { ParameterName = "endDate", Value = end };
                command.Parameters.Add(startDateParam);
                command.Parameters.Add(endDateParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<PolicyInformation> list = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PolicyInformation>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return list;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

                idbContext.DatabaseInstance.Connection.Close();
        public List<FailedMorniRequests> GetAllFailedMorniRequests(out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllFailedMorniRequests";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<FailedMorniRequests> list = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<FailedMorniRequests>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return list;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public List<string> GetAssignedDriverNinsByEmail(string email, out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAssignedDriverNinByEmail_SP";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter emailParam = new SqlParameter() { ParameterName = "email", Value = email };
                command.Parameters.Add(emailParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<string>(reader).ToList();

                if (data == null)
                {
                    dbContext.DatabaseInstance.Connection.Close();
                    return null;
                }

                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public CheckoutDetail GetCheckoutDetails(string referenceId)
        {
            return _checkOutDetailsRepository.Table.Where(c => c.ReferenceId == referenceId).FirstOrDefault();
        }
        public CheckoutDetail GetFromCheckoutDetailsByReferenceId (string referenceId,out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetFromCheckoutByReferenceId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter referenceIdParam = new SqlParameter() { ParameterName = "@referenceId", Value = referenceId };
                command.Parameters.Add(referenceIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var checkoutDetail = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetail>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return checkoutDetail;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public int UpdateCheckoutWithPaymentStatus(string referenceId, int policyStatusId, int paymentMethodId, out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "UpdateCheckoutWithPaymentStatus";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;
                SqlParameter referenceIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                SqlParameter policyStatusIdParam = new SqlParameter() { ParameterName = "policyStatusId", Value = policyStatusId };
                SqlParameter paymentMethodIdParam = new SqlParameter() { ParameterName = "paymentMethodId", Value = paymentMethodId };
                command.Parameters.Add(referenceIdParam);
                command.Parameters.Add(policyStatusIdParam);
                command.Parameters.Add(paymentMethodIdParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }
                exception = ex.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return -1;
            }
        }

        public CheckoutDetail GetLastPruchasedCheckoutDetailsByNIN(string driverNin, out string exception)

                idbContext.DatabaseInstance.Connection.Close();
        public Policy GetOLdTplPolicyData(string driverNin, string vehicleId, out string exception)
        public List<InsuredPolicyInfo> GetUserSuccessPoliciesDetails(string driverNin, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetUserSuccessPoliciesDetails";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "@DriverNin", Value = driverNin };
                command.Parameters.Add(userIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<InsuredPolicyInfo> insuredPolicies = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<InsuredPolicyInfo>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return insuredPolicies;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public List<InsuredPolicyDetails> GetUserSuccessPoliciesInfo(string driverNin, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetUserSuccessPoliciesInfo";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "@DriverNin", Value = driverNin };
                command.Parameters.Add(userIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<InsuredPolicyDetails> insuredPolicies = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<InsuredPolicyDetails>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return insuredPolicies;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }


        public int GetUserValidSuccessPolicies(string driverNin, out string exception)
        #endregion

        public CheckoutDetail GetUserActivePoliciesByNin(string driverNin, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetUserActivePoliciesByNin";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "@DriverNin", Value = driverNin };
                command.Parameters.Add(userIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                CheckoutDetail checkoutDetail = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetail>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return checkoutDetail;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public CorporateModel GetUserSuccessPoliciesDetailsForCorprate(string driverNin, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                CorporateModel corporateModel = new CorporateModel();
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetUserSuccessPoliciesDetailsForCorprate";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;

                SqlParameter userIdParam = new SqlParameter() { ParameterName = "@DriverNin", Value = driverNin };
                command.Parameters.Add(userIdParam);

                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                corporateModel.InsuredPolicies = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<InsuredPolicyInfo>(reader).ToList();
                reader.NextResult();
                corporateModel.EdaatRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatRequestModel>(reader).FirstOrDefault();
                return corporateModel;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
            finally
            {
                if(idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
        }

        public bool CheckIfQuotationIsRenewalByReferenceId(string referenceId, out string exception)
        {
            IDbContext dbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "CheckIfQuotationIsRenewalByReferenceId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter referenceIdParam = new SqlParameter() { ParameterName = "@referenceId", Value = referenceId };
                command.Parameters.Add(referenceIdParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                return ((IObjectContextAdapter)dbContext).ObjectContext.Translate<bool>(reader).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
    }
}