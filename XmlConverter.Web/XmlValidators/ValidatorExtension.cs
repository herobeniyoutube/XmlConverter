namespace XmlConverter.Web.XmlValidators
{
    public static class ValidatorExtension
    {
        public static IReadOnlyList<string> GetSchemas(string rootLevelDir, string xmlSpecificType, string fileType)
        {
            var xsdDir = Path.Combine(AppContext.BaseDirectory, rootLevelDir, xmlSpecificType);
            var schemas = Directory.GetFileSystemEntries(xsdDir, fileType, SearchOption.TopDirectoryOnly);

            return schemas;
        }
    }
}
