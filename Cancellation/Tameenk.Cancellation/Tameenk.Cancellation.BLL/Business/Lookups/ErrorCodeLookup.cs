using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tameenk.Cancellation.BLL.Caching;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.BLL.Business
{
    public class ErrorCodeLookup : IErrorCodeLookup
    {
        protected readonly IUnitOfWork unitOfWork;
        private readonly ICachingEngine cachingEngine;

        public ErrorCodeLookup(IUnitOfWork unitOfWork, ICachingEngine cachingEngine)
        {
            this.unitOfWork = unitOfWork;
            this.cachingEngine = cachingEngine;
        }

        public void Add(ErrorCode entity)
        {
            unitOfWork.ErrorCodes.Add(entity);
            unitOfWork.SaveChanges();
            var list = cachingEngine.GetErrorCodes();
            if (list == null) list = new List<ErrorCode>();
            list.Add(entity);
            cachingEngine.AddErrorCodes(list);
        }

        public void Update(ErrorCode entity)
        {
            unitOfWork.ErrorCodes.Update(entity);
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
            return this.unitOfWork.ErrorCodes.Count();
        }

        public List<ErrorCode> Find(Expression<Func<ErrorCode, bool>> predicate)
        {
            return this.unitOfWork.ErrorCodes.Find(predicate).ToList();
        }

        public ErrorCode Get(int id)
        {
            return this.unitOfWork.ErrorCodes.Get(id);
        }

        public List<ErrorCode> GetAll()
        {
            var list = this.cachingEngine.GetErrorCodes();
            if (list == null)
            {
                list = this.unitOfWork.ErrorCodes.GetAll().ToList();
                this.cachingEngine.AddErrorCodes(list);
            }
            return list;
        }

        public List<ErrorCode> GetActive()
        {
            var list = GetAll().Where(x => x.IsActive).ToList();
            return list;
        }

        public List<ErrorCode> GetInactive()
        {
            var list = GetAll().Where(x => x.IsActive == false).ToList();
            return list;
        }

        public void UpdateEntityInMemoery(ErrorCode entity)
        {
            var list = this.cachingEngine.GetErrorCodes();
            var deletedItem = list.FirstOrDefault(x => x.Code == entity.Code);
            list.Remove(deletedItem);
            list.Add(entity);
            this.cachingEngine.AddErrorCodes(list);
        }



    }
}
