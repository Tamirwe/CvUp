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
        public department AddDepartment(IdNameModel data, int companyId)
        {
            department newRec = _cvsPositionsQueries.AddDepartment(data, companyId);
            return newRec;
        }

        public department? UpdateDepartment(IdNameModel data, int companyId)
        {
            department? newRec = _cvsPositionsQueries.UpdateDepartment(data, companyId);
            return newRec;
        }

        public List<IdNameModel> GetDepartments(int companyId)
        {
            List<IdNameModel> depList = _cvsPositionsQueries.GetDepartments(companyId);
            return depList;
        }

        public department? DeleteDepartment(int companyId, int id)
        {
            var result = _cvsPositionsQueries.DeleteDepartment(companyId, id);
            return result;
        }

        public hr_company AddHrCompany(IdNameModel data, int companyId)
        {
            hr_company newRec = _cvsPositionsQueries.AddHrCompany(data, companyId);
            return newRec;
        }

        public hr_company? UpdateHrCompany(IdNameModel data, int companyId)
        {
            hr_company? newRec = _cvsPositionsQueries.UpdateHrCompany(data, companyId);
            return newRec;
        }

        public List<IdNameModel> GetHrCompanies(int companyId)
        {
            List<IdNameModel> depList = _cvsPositionsQueries.GetHrCompanies(companyId);
            return depList;
        }

        public hr_company? DeleteHrCompany(int companyId, int id)
        {
            var result = _cvsPositionsQueries.DeleteHrCompany(companyId, id);
            return result;
        }
    }
}
