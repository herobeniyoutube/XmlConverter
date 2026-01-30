using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XmlConverter.Web.XsdValidators.EmployersData
{
    public static class EmployersDataValidator
    {
        public static async Task<EmployersDataType> ValidateAsync(Stream xmlStream, CancellationToken cancellationToken = default)
        {
            var xsdDir = Path.Combine(AppContext.BaseDirectory, "XsdValidators", "EmployersData");
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
                        "Data1" => EmployersDataType.Data1,
                        "Data2" => EmployersDataType.Data2,
                        _ => throw new Exception("Schema type not found")
                    };
                }
            }

            throw new Exception("Incorrect input format");
        }
    }
}

