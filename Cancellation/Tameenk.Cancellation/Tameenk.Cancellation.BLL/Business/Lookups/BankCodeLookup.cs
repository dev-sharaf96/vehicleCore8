using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tameenk.Cancellation.BLL.Caching;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.BLL.Business
{
    public class BankCodeLookup : IBankCodeLookup
    {
        protected readonly IUnitOfWork unitOfWork;
        private readonly ICachingEngine cachingEngine;

        public BankCodeLookup(IUnitOfWork unitOfWork, ICachingEngine cachingEngine)
        {
            this.unitOfWork = unitOfWork;
            this.cachingEngine = cachingEngine;
        }

        public void Add(BankCode entity)
        {
            unitOfWork.BankCodes.Add(entity);
            unitOfWork.SaveChanges();
            var list = cachingEngine.GetBankCodes();
            if (list == null) list = new List<BankCode>();
            list.Add(entity);
            cachingEngine.AddBankCodes(list);
        }

        public void Update(BankCode entity)
        {
            unitOfWork.BankCodes.Update(entity);
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
            return this.unitOfWork.BankCodes.Count();
        }

        public List<BankCode> Find(Expression<Func<BankCode, bool>> predicate)
        {
            return this.unitOfWork.BankCodes.Find(predicate).ToList();
        }

        public BankCode Get(int id)
        {
            return this.unitOfWork.BankCodes.Get(id);
        }

        public List<BankCode> GetAll()
        {
            var list = this.cachingEngine.GetBankCodes();
            if (list == null)
            {
                list = this.unitOfWork.BankCodes.GetAll().ToList();
                this.cachingEngine.AddBankCodes(list);
            }
            return list;
        }

        public List<BankCode> GetActive()
        {
            var list = GetAll().Where(x => x.IsActive).ToList();
            return list;
        }

        public List<BankCode> GetInactive()
        {
            var list = GetAll().Where(x => x.IsActive == false).ToList();
            return list;
        }

        public void UpdateEntityInMemoery(BankCode entity)
        {
            var list = this.cachingEngine.GetBankCodes();
            var deletedItem = list.FirstOrDefault(x => x.Code == entity.Code);
            list.Remove(deletedItem);
            list.Add(entity);
            this.cachingEngine.AddBankCodes(list);
        }



    }
}
