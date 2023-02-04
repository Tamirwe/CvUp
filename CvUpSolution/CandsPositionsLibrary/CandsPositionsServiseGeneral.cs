using Database.models;
using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandsPositionsLibrary
{
    public partial class CandsPositionsServise
    {
        public department AddDepartment(IdNameModel data, int companyId)
        {
            department newRec = _cvsPositionsQueries.AddDepartment(data, companyId);
            return newRec;
        }

        public department? UpdateDepartment(IdNameModel data, int companyId)
        {
            department? updRec = _cvsPositionsQueries.UpdateDepartment(data, companyId);
            return updRec;
        }

        public List<IdNameModel> GetDepartmentsList(int companyId)
        {
            List<IdNameModel> depList = _cvsPositionsQueries.GetDepartmentsList(companyId);
            return depList;
        }

        public void DeleteDepartment(int companyId, int id)
        {
            _cvsPositionsQueries.DeleteDepartment(companyId, id);
        }

        public hr_company AddHrCompany(IdNameModel data, int companyId)
        {
            hr_company newRec = _cvsPositionsQueries.AddHrCompany(data, companyId);
            return newRec;
        }

        public hr_company? UpdateHrCompany(IdNameModel data, int companyId)
        {
            hr_company? updRec = _cvsPositionsQueries.UpdateHrCompany(data, companyId);
            return updRec;
        }

        public List<IdNameModel> GetHrCompaniesList(int companyId)
        {
            List<IdNameModel> depList = _cvsPositionsQueries.GetHrCompaniesList(companyId);
            return depList;
        }

        public void DeleteHrCompany(int companyId, int id)
        {
            _cvsPositionsQueries.DeleteHrCompany(companyId, id);
        }
    }
}
