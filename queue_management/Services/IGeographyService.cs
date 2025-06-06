using queue_management.Models;
using System.Threading.Tasks;

namespace queue_management.Services
{
    public interface IGeographyService
    {
        Task<(int CountryId, int DepartmentId, int MunicipalityId)> GetDefaultSelections();
        Task SetDefaultCountry(int countryId);
        Task SetDefaultDepartment(int departmentId);
        Task SetDefaultMunicipality(int MunicipalityId);
    }
}
