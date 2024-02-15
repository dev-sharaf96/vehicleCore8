using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tameenk.Cancellation.BLL.Caching;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.BLL.Business
{
    public class ReasonLookup : IReasonLookup
    {
        protected readonly IUnitOfWork unitOfWork;
        private readonly ICachingEngine cachingEngine;

        public ReasonLookup(IUnitOfWork unitOfWork, ICachingEngine cachingEngine)
        {
            this.unitOfWork = unitOfWork;
            this.cachingEngine = cachingEngine;
        }

        public void Add(Reason entity)
        {
            unitOfWork.Reasons.Add(entity);
            unitOfWork.SaveChanges();
            var list = cachingEngine.GetReasons();
            if (list == null) list = new List<Reason>();
            list.Add(entity);
            cachingEngine.AddReasons(list);
        }

        public void Update(Reason entity)
        {
            unitOfWork.Reasons.Update(entity);
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
            return this.unitOfWork.Reasons.Count();
        }

        public List<Reason> Find(Expression<Func<Reason, bool>> predicate)
        {
            return this.unitOfWork.Reasons.Find(predicate).ToList();
        }

        public Reason Get(int id)
        {
            return this.unitOfWork.Reasons.Get(id);
        }

        public List<Reason> GetAll()
        {
            var list = this.cachingEngine.GetReasons();
            if (list == null)
            {
                list = this.unitOfWork.Reasons.GetAll().ToList();
                this.cachingEngine.AddReasons(list);
            }
            return list;
        }

        public List<Reason> GetActive()
        {
            var list = GetAll().Where(x => x.IsActive).ToList();
            return list;
        }

        public List<Reason> GetInactive()
        {
            var list = GetAll().Where(x => x.IsActive == false).ToList();
            return list;
        }

        public void UpdateEntityInMemoery(Reason entity)
        {
            var list = this.cachingEngine.GetReasons();
            var deletedItem = list.FirstOrDefault(x => x.Code == entity.Code);
            list.Remove(deletedItem);
            list.Add(entity);
            this.cachingEngine.AddReasons(list);
        }



    }
}
