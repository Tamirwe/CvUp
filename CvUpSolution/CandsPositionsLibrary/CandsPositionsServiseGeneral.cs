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

       

    }
}
