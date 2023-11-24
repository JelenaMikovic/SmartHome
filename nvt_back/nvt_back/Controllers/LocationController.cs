using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.Services.Interfaces;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class LocationController : Controller
    {
        private ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            this._locationService = locationService;
        }

        [HttpGet]
        [Authorize(Roles = "USER, ADMIN, SUPERADMIN")]
        public ActionResult<IEnumerable<LocationDTO>> GetAll()
        {
            return Ok(this._locationService.GetAll());
        }
    }
}
