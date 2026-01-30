using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using XmlConverter.Web.XmlValidators.EmployersData;

namespace XmlConverter.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConverterController(ILogger<ConverterController> logger, EmployeeDataInMemoryStorage storage, EmployersDataValidator validator) : ControllerBase
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

            var type = await validator.ValidateAsync(buffer, HttpContext.RequestAborted);
            buffer.Position = 0;

            var xdoc = XDocument.Load(buffer, LoadOptions.PreserveWhitespace);
            storage.ReplaceData(xdoc, type);

            return Ok("Document uploaded");
        }

        [HttpPost("getConverted")]
        public async Task<IActionResult> ConvertData()
        {
            var doc = storage.GetEmployeesData();

            return Ok(doc.ToString());
        }
    }
}
