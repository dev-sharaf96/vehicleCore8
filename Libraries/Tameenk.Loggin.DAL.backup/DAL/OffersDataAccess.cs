using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Loggin.DAL.Entities;

namespace Tameenk.Loggin.DAL
{
    public class OffersDataAccess
    {
        public static bool AddOffer(Offer offer)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    offer.CreatedDate = DateTime.Now;
                    offer.LastModifiedDate = DateTime.Now;
                    context.Offers.Add(offer);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {

                    }
                }
                return false;
            }
        }

        public static List<Offer> Offers()
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                  return  context.Offers.ToList();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {

                    }
                }
                return null;
            }
        }

        public static List<Offer> Offers(bool isActive)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    return context.Offers.Where(x=>x.IsActive == isActive).ToList();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {

                    }
                }
                return null;
            }
        }

    }
}
