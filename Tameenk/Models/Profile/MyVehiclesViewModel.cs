using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TameenkDAL.Models;

namespace Tameenk.Models
{
    public class MyVehiclesViewModel
    {
        public List<VehicleModel> VehiclesList { get; internal set; }
        public int VehiclesTotalCount { get; internal set; }

        public int CurrentPage { get; set; }
        public string Lang { get; set; }
    }
}