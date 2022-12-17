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

        public hr_company AddHrCompany(IdNameModel data)
        {
            hr_company newRec = _cvsPositionsQueries.AddHrCompany(data);
            return newRec;
        }

        public hr_company? UpdateHrCompany(IdNameModel data)
        {
            hr_company? newRec = _cvsPositionsQueries.UpdateHrCompany(data);
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
