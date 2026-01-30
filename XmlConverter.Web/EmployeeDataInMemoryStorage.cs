using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using XmlConverter.Web.Dto;
using XmlConverter.Web.XmlValidators.EmployersData;
using static XmlConverter.Web.XmlValidators.ValidatorExtension;

namespace XmlConverter.Web
{
    public class EmployeeDataInMemoryStorage
    {
        EmployeesData ConvertedData { get; init; } = new EmployeesData(true);
        XDocument? _data;
        public EmployeesDataType? EmployeesType { get; private set; }

        public XDocument GetEmployeesData()
        {
            if (ConvertedData.NeedRecalculate || ConvertedData.EmployeesDataXml is null)
            {
                if (_data is null || EmployeesType is null) throw new InvalidOperationException("empty data");

                var transfrom = LoadXslt(GetXlstPath());

                var output = new XDocument();
                using var reader = _data.CreateReader();
                using var writer = output.CreateWriter();
                transfrom.Transform(reader, writer);

                ConvertedData.EmployeesDataXml = output;
                ConvertedData.NeedRecalculate = false;
            }

            return ConvertedData.EmployeesDataXml;
        }

        public XDocument GetData()
        {
            return _data ?? throw new InvalidOperationException("empty data");
        }

        public void ReplaceData(XDocument doc, EmployeesDataType type)
        {
            EmployeesType = type;
            var updated = type == EmployeesDataType.Data1 ? AddSumElement(doc) : doc;
            _data = NormalizeDocument(updated);
            ConvertedData.EmployeesDataXml = null;
            ConvertedData.NeedRecalculate = true;
        }

        public void AddItemIfCorrectType(AppendItemRequest item)
        {
            if (_data is null) throw new InvalidOperationException("empty data");
            if (EmployeesType != EmployeesDataType.Data1) throw new InvalidOperationException("not supported");

            var pay = _data.Root!;

            var existing = pay.Elements("item").FirstOrDefault(x =>
                string.Equals((string?)x.Attribute("name"), item.Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals((string?)x.Attribute("surname"), item.Surname, StringComparison.OrdinalIgnoreCase) &&
                string.Equals((string?)x.Attribute("month"), item.Month, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                pay.Add(new XElement(
                    "item",
                    new XAttribute("name", item.Name),
                    new XAttribute("surname", item.Surname),
                    new XAttribute("amount", item.Amount),
                    new XAttribute("month", item.Month)));
            }
            else
            {
                var existingAmount = ParseAmount((string?)existing.Attribute("amount"));
                var newAmount = ParseAmount(item.Amount);
                var sum = existingAmount + newAmount;
                existing.SetAttributeValue("amount", sum.ToString());
            }

            _data = NormalizeDocument(AddSumElement(_data));
            ConvertedData.EmployeesDataXml = null;
            ConvertedData.NeedRecalculate = true;
        }

        private static XslCompiledTransform LoadXslt(string xlst)
        {
            var transform = new XslCompiledTransform();
            transform.Load(xlst);
            return transform;
        }

        private string GetXlstPath()
        {
            var parts = typeof(EmployersDataValidator).Namespace!.Split('.');
            var schemas = GetSchemas(parts[^2], parts[^1], "*.xslt");

            foreach (var schema in schemas)
            {
                var name = Path.GetFileNameWithoutExtension(schema);

                if (name == EmployeesType.ToString())
                {
                    return schema;
                }
            }

            throw new InvalidOperationException($"xlst not found for type {EmployeesType}");
        }

        private static XDocument AddSumElement(XDocument doc)
        {
            var pay = doc.Root!;
            pay.Elements("sum").Remove();

            var total = 0m;
            foreach (var item in pay.Elements("item"))
            {
                var amount = ParseAmount((string?)item.Attribute("amount"));
                total += amount;
            }

            pay.Add(new XElement("sum", new XAttribute("amount", total.ToString(CultureInfo.InvariantCulture))));
            return doc;
        }

        private static decimal ParseAmount(string? amountValue)
        {
            if (string.IsNullOrWhiteSpace(amountValue)) return 0m;

            var normalized = amountValue.Replace('.', ',');

            if (decimal.TryParse(normalized, out var amount)) return amount;

            throw new InvalidOperationException($"invalid amount value {amountValue}");
        }

        private class EmployeesData(bool isNew)
        {
            public XDocument? EmployeesDataXml { get; set; }
            public bool NeedRecalculate { get; set; } = isNew;
        }

        private static XDocument NormalizeDocument(XDocument doc)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = false
            };

            using var sw = new StringWriter(CultureInfo.InvariantCulture);
            using (var xw = XmlWriter.Create(sw, settings))
            {
                doc.Save(xw);
            }

            return XDocument.Parse(sw.ToString(), LoadOptions.None);
        }
    }
}
