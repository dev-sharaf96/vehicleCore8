using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tameenk.Cancellation.BLL.Caching;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.BLL.Business
{
    public class InsuranceTypeLookup : IInsuranceTypeLookup
    {
        protected readonly IUnitOfWork unitOfWork;
        private readonly ICachingEngine cachingEngine;

        public InsuranceTypeLookup(IUnitOfWork unitOfWork, ICachingEngine cachingEngine)
        {
            this.unitOfWork = unitOfWork;
            this.cachingEngine = cachingEngine;
        }

        public void Add(InsuranceType entity)
        {
            unitOfWork.InsuranceTypes.Add(entity);
            unitOfWork.SaveChanges();
            var list = cachingEngine.GetInsuranceTypes();
            if (list == null) list = new List<InsuranceType>();
            list.Add(entity);
            cachingEngine.AddInsuranceTypes(list);
        }

        public void Update(InsuranceType entity)
        {
            unitOfWork.InsuranceTypes.Update(entity);
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
            return this.unitOfWork.InsuranceTypes.Count();
        }

        public List<InsuranceType> Find(Expression<Func<InsuranceType, bool>> predicate)
        {
            return this.unitOfWork.InsuranceTypes.Find(predicate).ToList();
        }

        public InsuranceType Get(int id)
        {
            return this.unitOfWork.InsuranceTypes.Get(id);
        }

        public List<InsuranceType> GetAll()
        {
            var list = this.cachingEngine.GetInsuranceTypes();
            if (list == null)
            {
                list = this.unitOfWork.InsuranceTypes.GetAll().ToList();
                this.cachingEngine.AddInsuranceTypes(list);
            }
            return list;
        }

        public List<InsuranceType> GetActive()
        {
            var list = GetAll().Where(x => x.IsActive).ToList();
            return list;
        }

        public List<InsuranceType> GetInactive()
        {
            var list = GetAll().Where(x => x.IsActive == false).ToList();
            return list;
        }

        public void UpdateEntityInMemoery(InsuranceType entity)
        {
            var list = this.cachingEngine.GetInsuranceTypes();
            var deletedItem = list.FirstOrDefault(x => x.Code == entity.Code);
            list.Remove(deletedItem);
            list.Add(entity);
            this.cachingEngine.AddInsuranceTypes(list);
        }



    }
}
