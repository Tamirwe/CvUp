using Database.models;
using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvsPositionsLibrary
{
    public partial class CvsPositionsServise
    {
        public hr_company AddUpdateHrCompany(IdNameModel data)
        {
            hr_company newRec = _cvsPositionsQueries.AddNewHrCompany(data);
            return newRec;
        }

        public department AddDepartment(IdNameModel data)
        {
            department newRec = _cvsPositionsQueries.AddDepartment(data);
            return newRec;
        }

        public department? UpdateDepartment(IdNameModel data)
        {
            department? newRec = _cvsPositionsQueries.UpdateDepartment(data);
            return newRec;
        }

        public List<IdNameModel> GetCompanyDepartments(int companyId)
        {
            List<IdNameModel> depList = _cvsPositionsQueries.GetCompanyDepartments(companyId);
            return depList;
        }

        public department? DeleteCompanyDepartment(int companyId, int id)
        {
            var result = _cvsPositionsQueries.DeleteCompanyDepartment(companyId, id);
            return result;
        }


    }
}
