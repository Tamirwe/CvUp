using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{

    public class LoginQueries : ILoginQueries
    {
        private cvup00001Context dbContext;

        public LoginQueries()
        {
            dbContext = new cvup00001Context();
        }

        public List<user> getUsersByEmailPassword(string email, string password)
        {
            var usersList = dbContext.users.Where(x => x.email == email && x.passwaord == password && x.activate_status_id == (int)UserActivateStatus.ACTIVE).ToList();
            return usersList;
        }

        public List<IdNameModel> getUserCompanies(string email)
        {
            using (dbContext)
            {
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                string sql = @"SELECT c.id id, c.name
                        FROM users u INNER JOIN companies c ON u.company_id=c.id
                        WHERE u.email='" + email + "'" +
                        @"AND  u.activate_status_id = " + (int)UserActivateStatus.ACTIVE;

                var userCompaniesList = dbContext.idNameModelDB.FromSqlRaw(sql).ToList();
                return userCompaniesList;
            }
        }

        public List<user> getUsers(string email, int? companyId)
        {
            if (companyId != 0)
            {
                return dbContext.users.Where(x => x.email == email && x.company_id == companyId && x.activate_status_id == (int)UserActivateStatus.ACTIVE).ToList();
            }

            return dbContext.users.Where(x => x.email == email && x.activate_status_id == (int)UserActivateStatus.ACTIVE).ToList();
        }

    }
}
