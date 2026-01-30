using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;
using XmlConverter.Web.Dto;
using XmlConverter.Web.Services;
using XmlConverter.Web.XmlValidators.EmployersData;

namespace XmlConverter.Web.Pages
{
    public class EmployeesModel(ConverterService service) : PageModel
    {
        public string ConvertedXml { get; private set; } = string.Empty;
        public string SourceXml { get; private set; } = string.Empty;
        public EmployeesDataType? EmployeesType { get; private set; }
        public IReadOnlyList<EmployeeView> Employees { get; private set; } = Array.Empty<EmployeeView>();

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
            EmployeesType = service.GetEmployeesType();
            ConvertedXml = service.ConvertData();
            SourceXml = service.GetData();
            Employees = ParseEmployees(ConvertedXml);
        }

        private static IReadOnlyList<EmployeeView> ParseEmployees(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return Array.Empty<EmployeeView>();
            }

            var doc = XDocument.Parse(xml, LoadOptions.None);
            if (doc.Root is null)
            {
                return Array.Empty<EmployeeView>();
            }

            var result = new List<EmployeeView>();
            foreach (var employee in doc.Root.Elements("Employee"))
            {
                var name = (string?)employee.Attribute("name") ?? string.Empty;
                var surname = (string?)employee.Attribute("surname") ?? string.Empty;
                var salaries = employee.Elements("salary")
                    .Select(s => new SalaryView(
                        (string?)s.Attribute("month") ?? string.Empty,
                        (string?)s.Attribute("amount") ?? string.Empty))
                    .ToList();

                result.Add(new EmployeeView(name, surname, salaries));
            }

            return result;
        }

        public sealed record EmployeeView(string Name, string Surname, IReadOnlyList<SalaryView> Salaries);
        public sealed record SalaryView(string Month, string Amount);
    }
}
