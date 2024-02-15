using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tameenk.Cancellation.BLL.Caching;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.BLL.Business
{
    public class InsuranceCompanyBusiness : IInsuranceCompanyBusiness
    {
        protected readonly IUnitOfWork unitOfWork;
        private readonly ICachingEngine cachingEngine;

        public InsuranceCompanyBusiness(IUnitOfWork unitOfWork, ICachingEngine cachingEngine)
        {
            this.unitOfWork = unitOfWork;
            this.cachingEngine = cachingEngine;
        }

        public void Add(InsuranceCompany entity)
        {
            unitOfWork.InsuranceCompanies.Add(entity);
            unitOfWork.SaveChanges();
            var list = cachingEngine.GetInsuranceCompanies();
            if (list == null) list = new List<InsuranceCompany>();
            list.Add(entity);
            cachingEngine.AddInsuranceCompanies(list);
        }

        public void Update(InsuranceCompany entity)
        {
            unitOfWork.InsuranceCompanies.Update(entity);
            unitOfWork.SaveChanges();
            UpdateEntityInMemoery(entity);
        }

        public void Remove(int Id)
        {
            var entity = Get(Id);
            entity.IsActive = false;
            Update(entity);
        }

        public int Count()
        {
            return this.unitOfWork.InsuranceCompanies.Count();
        }

        public List<InsuranceCompany> Find(Expression<Func<InsuranceCompany, bool>> predicate)
        {
            return this.unitOfWork.InsuranceCompanies.Find(predicate).ToList();
        }

        public InsuranceCompany Get(int id)
        {
            return this.unitOfWork.InsuranceCompanies.Get(id);
        }

        public List<InsuranceCompany> GetAll()
        {
            var list = this.cachingEngine.GetInsuranceCompanies();
            if (list == null)
            {
                list = this.unitOfWork.InsuranceCompanies.GetAll().ToList();
                this.cachingEngine.AddInsuranceCompanies(list);
            }
            return list;
        }

        public List<InsuranceCompany> GetActive()
        {
            var list = GetAll().Where(x => x.IsActive).ToList();
            return list;
        }

        public List<InsuranceCompany> GetInactive()
        {
            var list = GetAll().Where(x => x.IsActive == false).ToList();
            return list;
        }

        public void UpdateEntityInMemoery(InsuranceCompany entity)
        {
            var list = this.cachingEngine.GetInsuranceCompanies();
            var deletedItem = list.FirstOrDefault(x => x.Id == entity.Id);
            list.Remove(deletedItem);
            list.Add(entity);
            this.cachingEngine.AddInsuranceCompanies(list);
        }



    }
}
