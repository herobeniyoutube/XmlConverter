using System.IO;
using Microsoft.AspNetCore.Mvc;
using XmlConverter.Web.XsdValidators.EmployersData;

namespace XmlConverter.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConverterController(ILogger<ConverterController> logger) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            if (Request.ContentLength == null || Request.ContentLength == 0)
            {
                return BadRequest("Empty body");
            }

            await using var buffer = new MemoryStream();
            await Request.Body.CopyToAsync(buffer, HttpContext.RequestAborted);
            buffer.Position = 0;

            var result = await EmployersDataValidator.ValidateAsync(buffer, HttpContext.RequestAborted);


            return BadRequest("Empty body");

        }
    }
}
