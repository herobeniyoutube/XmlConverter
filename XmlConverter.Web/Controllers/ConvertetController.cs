using Microsoft.AspNetCore.Mvc;

namespace XmlConverter.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertetController(ILogger<ConvertetController> logger) : ControllerBase
    {
        [HttpPost]
        public IEnumerable<WeatherForecast> Convert()
        {
            
        }
    }
}
