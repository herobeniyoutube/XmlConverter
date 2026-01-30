using XmlConverter.Web.Dto;
using XmlConverter.Web.XmlValidators.EmployersData;

namespace XmlConverter.Web.Abstractions
{
    public interface IConverterService
    {
        Task Upload(Stream reader, CancellationToken cancellationToken = default);
        string ConvertData();
        string GetData();
        EmployeesDataType? GetEmployeesType();
        void AppendData(AppendItemRequest request);
    }
}
