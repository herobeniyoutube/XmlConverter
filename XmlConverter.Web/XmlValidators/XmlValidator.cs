using System.Xml;
using System.Xml.Schema;

namespace XmlConverter.Web.XmlValidators
{
    public static class XmlValidator
    {
        public static async Task<IReadOnlyList<string>> ValidateXmlAsync(Stream xmlStream, string xsdPath, CancellationToken cancellationToken = default)
        {
            var errors = new List<string>();

            var schemas = new XmlSchemaSet();
            schemas.Add(null, xsdPath);

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemas,
                DtdProcessing = DtdProcessing.Prohibit,
                Async = true,
            };

            settings.ValidationEventHandler += (s, e) =>
            {
                errors.Add($"{e.Severity}: {e.Message}");
            };

            using var reader = XmlReader.Create(xmlStream, settings);
            while (await reader.ReadAsync()) cancellationToken.ThrowIfCancellationRequested();

            return errors;
        }
    }
}


