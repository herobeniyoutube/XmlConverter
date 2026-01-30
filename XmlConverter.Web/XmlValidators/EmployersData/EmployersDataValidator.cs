using XmlConverter.Web.Abstractions;

namespace XmlConverter.Web.XmlValidators.EmployersData
{
    public class EmployersDataValidator : XmlValidator, IEmployersDataValidator
    {
        public override string GetNameFromNamespace(int indexFromEnd)
        {
            var ns = typeof(EmployersDataValidator).Namespace!;
            var parts = ns.Split('.');

            return parts[^indexFromEnd];
        }

        public  async Task<EmployeesDataType> ValidateAsync(Stream xmlStream, CancellationToken cancellationToken = default)
        {
            var schemas = ValidatorExtension.GetSchemas(GetNameFromNamespace(2), GetNameFromNamespace(1), "*.xsd");

            if (schemas.Count == 0)
            {
                throw new Exception("Incorrect input format");
            }

            using var buffer = new MemoryStream();
            await xmlStream.CopyToAsync(buffer, cancellationToken);
            buffer.Position = 0;

            foreach (var schema in schemas)
            {
                buffer.Position = 0;
                var errors = await ValidateXmlAsync(buffer, schema, cancellationToken);
                if (errors.Count == 0)
                {
                    var name = Path.GetFileNameWithoutExtension(schema);
                    return name switch
                    {
                        "Data1" => EmployeesDataType.Data1,
                        "Data2" => EmployeesDataType.Data2,
                        _ => throw new Exception("Schema type not found")
                    };
                }
            }

            throw new Exception("Incorrect input format");
        }
    }
}

