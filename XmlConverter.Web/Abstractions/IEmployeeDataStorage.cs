using System.Xml.Linq;
using XmlConverter.Web.Dto;
using XmlConverter.Web.XmlValidators.EmployersData;

namespace XmlConverter.Web.Abstractions
{
    public interface IEmployeeDataStorage
    {
        EmployeesDataType? EmployeesType { get; }
        XDocument GetEmployeesData();
        XDocument GetData();
        void ReplaceData(XDocument doc, EmployeesDataType type);
        void AddItemIfCorrectType(AppendItemRequest item);
    }
}
