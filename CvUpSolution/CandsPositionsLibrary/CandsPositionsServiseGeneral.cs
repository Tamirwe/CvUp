using Database.models;
using DataModelsLibrary.Models;

namespace CandsPositionsLibrary
{
    public partial class CandsPositionsServise
    {
        public async Task<department> AddDepartment(IdNameModel data, int companyId)
        {
            department newRec = await _cvsPositionsQueries.AddDepartment(data, companyId);
            return newRec;
        }

        public async Task<department?> UpdateDepartment(IdNameModel data, int companyId)
        {
            department? updRec = await _cvsPositionsQueries.UpdateDepartment(data, companyId);
            return updRec;
        }

        public async Task<List<IdNameModel>> GetDepartmentsList(int companyId)
        {
            List<IdNameModel> depList = await _cvsPositionsQueries.GetDepartmentsList(companyId);
            return depList;
        }

        public async Task DeleteDepartment(int companyId, int id)
        {
            await _cvsPositionsQueries.DeleteDepartment(companyId, id);
        }

        public async Task<hr_company> AddHrCompany(IdNameModel data, int companyId)
        {
            hr_company newRec = await _cvsPositionsQueries.AddHrCompany(data, companyId);
            return newRec;
        }

        public async Task<hr_company?> UpdateHrCompany(IdNameModel data, int companyId)
        {
            hr_company? updRec = await _cvsPositionsQueries.UpdateHrCompany(data, companyId);
            return updRec;
        }

        public async Task<List<IdNameModel>> GetHrCompaniesList(int companyId)
        {
            List<IdNameModel> depList = await _cvsPositionsQueries.GetHrCompaniesList(companyId);
            return depList;
        }

        public async Task DeleteHrCompany(int companyId, int id)
        {
            await _cvsPositionsQueries.DeleteHrCompany(companyId, id);
        }
    }
}
