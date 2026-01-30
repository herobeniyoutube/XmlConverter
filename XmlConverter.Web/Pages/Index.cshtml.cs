using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XmlConverter.Web.Services;

namespace XmlConverter.Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel(ConverterService service, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment) : PageModel
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Enviroment = environment;
        public string Error { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostUpload(IFormFile postedFile)
        {
            if (postedFile is null || postedFile.Length == 0)
            {
                Error = "posted file is empty";
                return Page();
            }

            var stream = postedFile.OpenReadStream();
            await service.Upload(stream);

            return RedirectToPage("/Employees");
        }
    }
}
