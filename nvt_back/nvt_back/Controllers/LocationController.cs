using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.Services.Interfaces;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class LocationController : ControllerBase
    {
        private ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            this._locationService = locationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<LocationDTO>> GetAll()
        {
            return Ok(this._locationService.GetAll());
        }
    }
}
