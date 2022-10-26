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

    public class AuthQueries : IAuthQueries
    {
        private cvup00001Context dbContext;

        public AuthQueries()
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

        public company AddNewCompany(string companyName, string? companyDescr, CompanyActivateStatus status)
        {
            var company = new company
            {
                name = companyName,
                descr = companyDescr,
                activate_status_id = (int)status,
            };

            dbContext.companies.Add(company);
            dbContext.SaveChanges();
            return company;
        }

        public user AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActivateStatus status, UsersRole role, string log)
        {
            var user = new user
            {
                company_id = companyId,
                email = email,
                passwaord = password,
                first_name = firstName,
                last_name = lastName,
                activate_status_id = (int)status,
                role = (int)role,
                log_info = log
            };

            dbContext.users.Add(user);
            dbContext.SaveChanges();
            return user;
        }

        public company updateCompany(company _company)
        {
            var result = dbContext.companies.Update(_company);
            dbContext.SaveChanges();
            return _company;
        }

        public void addUserPasswordReset(string key, user user)
        {
            var pr = new password_reset
            {
                email = user.email,
                user_id = user.id,
                key = key,
            };

            dbContext.password_resets.Add(pr);
            dbContext.SaveChanges();
        }
    }
}
