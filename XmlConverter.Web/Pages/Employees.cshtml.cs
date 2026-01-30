using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XmlConverter.Web.Dto;
using XmlConverter.Web.Services;

namespace XmlConverter.Web.Pages
{
    public class EmployeesModel(ConverterService service) : PageModel
    {
        public string ConvertedXml { get; private set; } = string.Empty;
        public string SourceXml { get; private set; } = string.Empty;

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string Surname { get; set; } = string.Empty;

        [BindProperty]
        public string Amount { get; set; } = string.Empty;

        [BindProperty]
        public string Month { get; set; } = string.Empty;

        public void OnGet()
        {
            LoadXml(service);
        }

        public IActionResult OnPostAppend()
        {
            service.AppendData(new AppendItemRequest(Name, Surname, Amount, Month));
            LoadXml(service);
            return Page();
        }

        private void LoadXml(ConverterService service)
        {
            ConvertedXml = service.ConvertData();
            SourceXml = service.GetData();
        }
    }
}
