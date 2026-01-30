namespace XmlConverter.Web.XmlValidators.EmployersData
{
    public static class EmployersDataValidator
    {
        public static async Task<EmployeesDataType> ValidateAsync(Stream xmlStream, CancellationToken cancellationToken = default)
        {
            var xsdDir = Path.Combine(AppContext.BaseDirectory, "XmlValidators", "EmployersData");
            var schemas = Directory.GetFileSystemEntries(xsdDir, "*.xsd", SearchOption.TopDirectoryOnly);

            if (schemas.Length == 0)
            {
                throw new Exception("Incorrect input format");
            }

            using var buffer = new MemoryStream();
            await xmlStream.CopyToAsync(buffer, cancellationToken);
            buffer.Position = 0;

            foreach (var schema in schemas)
            {
                buffer.Position = 0;
                var errors = await XmlValidator.ValidateXmlAsync(buffer, schema, cancellationToken);
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

