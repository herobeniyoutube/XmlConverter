using XmlConverter.Web.XmlValidators.EmployersData;

namespace XmlConverter.Web.Abstractions
{
    public interface IEmployersDataValidator
    {
        Task<EmployeesDataType> ValidateAsync(Stream xmlStream, CancellationToken cancellationToken = default);
    }
}
