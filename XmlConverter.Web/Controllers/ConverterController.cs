using Microsoft.AspNetCore.Mvc;
using XmlConverter.Web.Dto;
using XmlConverter.Web.Services;

namespace XmlConverter.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConverterController(ConverterService service) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            if (Request.ContentLength == null || Request.ContentLength == 0)
            {
                return BadRequest("Empty body");
            }

            await service.Upload(Request.Body, HttpContext.RequestAborted);

            return Ok("Document uploaded");
        }

        [HttpGet("converted")]
        public IActionResult ConvertData()
        {
            return Ok(service.ConvertData());
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            return Ok(service.GetData());
        }

        [HttpPost("append_data")]
        public IActionResult AppendData([FromQuery] AppendItemRequest request)
        {
            service.AppendData(request);

            return Ok("Item appended");
        }
    }
}
