using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tameenk.Cancellation.BLL.Business;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleIDTypesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IVehicleIDTypeLookup VehicleIDTypeLookup;

        public VehicleIDTypesController(IUnitOfWork unitOfWork, IVehicleIDTypeLookup VehicleIDTypeLookup)
        {
            this.unitOfWork = unitOfWork;
            this.VehicleIDTypeLookup = VehicleIDTypeLookup;
        }


        // POST api/VehicleIDTypes
        [HttpPost]
        public IActionResult Post([FromBody] VehicleIDType VehicleIDType)
        {
            VehicleIDTypeLookup.Add(VehicleIDType);
            return Ok(VehicleIDType);
        }

        // PUT api/VehicleIDTypes
        [HttpPut]
        public IActionResult Put([FromBody] VehicleIDType VehicleIDType)
        {
            VehicleIDTypeLookup.Update(VehicleIDType);
            return Ok(VehicleIDType);
        }

        // DELETE api/VehicleIDTypes/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            VehicleIDTypeLookup.Remove(id);
            return Ok();
        }

        // GET api/VehicleIDTypes
        [HttpGet]
        public ActionResult<IEnumerable<VehicleIDType>> GetVehicleIDTypes()
        {
            return Ok(VehicleIDTypeLookup.GetAll());
        }
    }
}