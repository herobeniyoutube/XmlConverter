using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using XmlConverter.Web.Dto;
using XmlConverter.Web.XmlValidators.EmployersData;

namespace XmlConverter.Web.Services
{
    public class ConverterService(EmployeeDataInMemoryStorage storage, EmployersDataValidator validator)
    {
        public async Task Upload(Stream reader, CancellationToken cancellationToken = default)
        {
            await using var buffer = new MemoryStream();
            await reader.CopyToAsync(buffer, cancellationToken);
            buffer.Position = 0;

            var type = await validator.ValidateAsync(buffer, cancellationToken);
            buffer.Position = 0;

            var xdoc = XDocument.Load(buffer, LoadOptions.PreserveWhitespace);
            storage.ReplaceData(xdoc, type);
        }

        public string ConvertData()
        {
            var doc = storage.GetEmployeesData();

            return doc.ToString().ToString();
        }

        public string GetData()
        {
            return storage.GetData().ToString();
        }

        public void AppendData(AppendItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Surname) ||
                string.IsNullOrWhiteSpace(request.Amount) ||
                string.IsNullOrWhiteSpace(request.Month))
            {
                throw new InvalidOperationException("Missing required fields");
            }

            var item = new XElement(
                "item",
                new XAttribute("name", request.Name),
                new XAttribute("surname", request.Surname),
                new XAttribute("amount", request.Amount),
                new XAttribute("month", request.Month));

            storage.AddItemIfCorrectType(item);
        }
    }
}
