using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Quotations;

namespace Tameenk.Core.Domain.Enums.Quotations
{
    /// <summary>
    /// Driving violation
    /// </summary>
    public enum Violation
    {
        /// <summary>
        /// Speed Ticket
        /// </summary>
        [LocalizedName(typeof(ViolationResource), "SpeedTicket")]
        SpeedTicket = 1,
        /// <summary>
        /// Override Traffic Light
        /// </summary>
        [LocalizedName(typeof(ViolationResource), "OverrideTrafficLight")]
        OverrideTrafficLight = 2,
        /// <summary>
        /// Driving Opposite Direction
        /// </summary>
        [LocalizedName(typeof(ViolationResource), "DrivingOppositeDirection")]
        DrivingOppositeDirection = 3,
        /// <summary>
        /// Drifting
        /// </summary>
        [LocalizedName(typeof(ViolationResource), "Drifting")]
        Drifting = 4,
        /// <summary>
        /// Parking Violation
        /// </summary>
        [LocalizedName(typeof(ViolationResource), "ParkingViolation")]
        ParkingViolation = 5

    }
}
