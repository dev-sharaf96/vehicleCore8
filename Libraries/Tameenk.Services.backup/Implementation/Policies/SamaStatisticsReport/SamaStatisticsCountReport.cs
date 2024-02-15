using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Implementation.Policies
{
   public class SamaStatisticsCountReport
    {
        public List<TotalPolicesPerGenderModel> TotalPolicesPerGenderModel  { get;set ; }
        public List<TotalcontPerAgeForAllRanges> AllAgeRanges { get;set ; }
        public List<AllPaymentMethodModel> AllPaymentMethodModel { get;set ; }
        public List<TotalInSystemModel> totalInSystemModel   { get;set ; }
        public List<AllChannelModel> AllChannelModel  { get;set ; }
        public List<TotalPolicesPerCity> TotalIndividualPolicesPerCity { get;set ; }
        public List<TotalPolicesPerCity> TotalCorpratePolicesPerCity { get;set ; }
    }
}
