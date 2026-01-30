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

        [HttpGet("converted")]
        public IActionResult ConvertData()
        {
            var doc = storage.GetEmployeesData();

            return Ok(doc.ToString());
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            var doc = storage.GetData();

            return Ok(doc.ToString());
        }

        [HttpPost("append_data")]
        public IActionResult AppendData([FromQuery] AppendItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Surname) ||
                string.IsNullOrWhiteSpace(request.Amount) ||
                string.IsNullOrWhiteSpace(request.Month))
            {
                return BadRequest("Missing required fields");
            }

            var item = new XElement(
                "item",
                new XAttribute("name", request.Name),
                new XAttribute("surname", request.Surname),
                new XAttribute("amount", request.Amount),
                new XAttribute("month", request.Month));

            storage.AddItemIfCorrectType(item);

            return Ok("Item appended");
        }

        public sealed record AppendItemRequest(string Name, string Surname, string Amount, string Month);
    }
}
