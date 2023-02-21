using Database.models;
using DataModelsLibrary.Models;

namespace CandsPositionsLibrary
{
    public partial class CandsPositionsServise
    {
        public async Task<customer> AddCustomer(IdNameModel data, int companyId)
        {
            customer newRec = await _cvsPositionsQueries.AddCustomer(data, companyId);
            return newRec;
        }

        public async Task<customer?> UpdateCustomer(IdNameModel data, int companyId)
        {
            customer? updRec = await _cvsPositionsQueries.UpdateCustomer(data, companyId);
            return updRec;
        }

        public async Task<List<IdNameModel>> GetCustomersList(int companyId)
        {
            List<IdNameModel> depList = await _cvsPositionsQueries.GetCustomersList(companyId);
            return depList;
        }

        public async Task DeleteCustomer(int companyId, int id)
        {
            await _cvsPositionsQueries.DeleteCustomer(companyId, id);
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
