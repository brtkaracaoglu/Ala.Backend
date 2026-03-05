using Microsoft.AspNetCore.Mvc;

namespace Ala.Backend.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new { Message = "Ala Backend başarıyla çalışıyor!", Time = DateTime.Now });
        }
    }
}