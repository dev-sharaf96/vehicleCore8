using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tameenk.Cancellation.BLL.Caching;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.BLL.Business
{
    public class VehicleIDTypeLookup : IVehicleIDTypeLookup
    {
        protected readonly IUnitOfWork unitOfWork;
        private readonly ICachingEngine cachingEngine;

        public VehicleIDTypeLookup(IUnitOfWork unitOfWork, ICachingEngine cachingEngine)
        {
            this.unitOfWork = unitOfWork;
            this.cachingEngine = cachingEngine;
        }

        public void Add(VehicleIDType entity)
        {
            unitOfWork.VehicleIDTypes.Add(entity);
            unitOfWork.SaveChanges();
            var list = cachingEngine.GetVehicleIDTypes();
            if (list == null) list = new List<VehicleIDType>();
            list.Add(entity);
            cachingEngine.AddVehicleIDTypes(list);
        }

        public void Update(VehicleIDType entity)
        {
            unitOfWork.VehicleIDTypes.Update(entity);
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
            return this.unitOfWork.VehicleIDTypes.Count();
        }

        public List<VehicleIDType> Find(Expression<Func<VehicleIDType, bool>> predicate)
        {
            return this.unitOfWork.VehicleIDTypes.Find(predicate).ToList();
        }

        public VehicleIDType Get(int id)
        {
            return this.unitOfWork.VehicleIDTypes.Get(id);
        }

        public List<VehicleIDType> GetAll()
        {
            var list = this.cachingEngine.GetVehicleIDTypes();
            if (list == null)
            {
                list = this.unitOfWork.VehicleIDTypes.GetAll().ToList();
                this.cachingEngine.AddVehicleIDTypes(list);
            }
            return list;
        }

        public List<VehicleIDType> GetActive()
        {
            var list = GetAll().Where(x => x.IsActive).ToList();
            return list;
        }

        public List<VehicleIDType> GetInactive()
        {
            var list = GetAll().Where(x => x.IsActive == false).ToList();
            return list;
        }

        public void UpdateEntityInMemoery(VehicleIDType entity)
        {
            var list = this.cachingEngine.GetVehicleIDTypes();
            var deletedItem = list.FirstOrDefault(x => x.Code == entity.Code);
            list.Remove(deletedItem);
            list.Add(entity);
            this.cachingEngine.AddVehicleIDTypes(list);
        }



    }
}
