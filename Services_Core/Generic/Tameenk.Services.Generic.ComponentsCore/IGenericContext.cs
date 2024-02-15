using System.Collections.Generic;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Generic.Components.Models;

namespace Tameenk.Services.Generic.Component
{
    public interface IGenericContext
    {
        #region Offers
        List<Offer> GetOffers();
        IPagedList<Offer> GetOffers(int pageIndx = 0, int pageSize = int.MaxValue);
        Offer AddOffer(Offer offer);
        Offer UpdateOffer(Offer offer);
        Offer ActivateDeActivateOffer(int Id, bool isDeleted);
        #endregion

        #region ContactUs
        ContactUs SaveContactUsRequest(ContactUs data);
        #endregion

        #region Career
        Career SaveCareerRequest(CareerModel data, string userId, out string exception);
        #endregion

    }
}
