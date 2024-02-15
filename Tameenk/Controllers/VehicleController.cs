using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TameenkDAL;

namespace Tameenk.Controllers
{
    public class VehicleController : Controller
    {
        TameenkDbContext _db = new TameenkDbContext();

        // GET: Vehicle
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Delete(string vehicleID)
        {
            if (string.IsNullOrEmpty(vehicleID))
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            DateTime dateTime = DateTime.Now;

            int canDeleteVehicle = (from p in _db.Policies
                                   join ch in _db.CheckoutDetails on p.CheckOutDetailsId equals ch.ReferenceId
                                   join v in _db.Vehicles on ch.VehicleId equals v.ID
                                   where p.PolicyExpiryDate <= dateTime && v.ID.ToString() == vehicleID
                                   select ch.Vehicle).Count();
            
            if(canDeleteVehicle == 0)
            {
                //get the obj from db and if it exist then delete it

                var vehicle = _db.Vehicles.Where(x => x.ID == new Guid(vehicleID)).FirstOrDefault();

                if (vehicle != null)
                {
                    vehicle.IsDeleted = true;
                   // _db.Entry(vehicle).State = EntityState.Modified;
                    _db.SaveChanges();

                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                
            }


            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

        }
    }
}