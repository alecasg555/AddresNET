using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoCodification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeoCodificationController : ControllerBase
    {
        private readonly IGeoCodificationService _geoCodificationService;

        public GeoCodificationController(IGeoCodificationService geoCodificationService)
        {
             _geoCodificationService = geoCodificationService;
        }

        public async Task<IActionResult> GetAddressCoordinates(string address)
        {
            string Coordinates = await _geoCodificationService.GetAddressCoordinates(address);
            return Ok(Coordinates);
        }
    }
}
