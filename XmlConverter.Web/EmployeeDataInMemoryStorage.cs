using System.Xml.Linq;
using System.Xml.Xsl;
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
                if (_data is null || EmployeesType is null)
                {
                    throw new InvalidOperationException("empty data");
                }

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
            _data = doc;
            ConvertedData.EmployeesDataXml = null;
            ConvertedData.NeedRecalculate = true;
        }

        public void AddItemIfCorrectType(XElement item)
        {
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

        private class EmployeesData(bool isNew)
        {
            public XDocument? EmployeesDataXml { get; set; }
            public bool NeedRecalculate { get; set; } = isNew;
        }
    }
}
