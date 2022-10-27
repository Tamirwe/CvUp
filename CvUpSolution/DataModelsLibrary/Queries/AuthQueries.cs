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
            using (var db = dbContext)
            {
                var usersList = db.users.Where(x => x.email == email && x.passwaord == password && x.activate_status_id == (int)UserActivateStatus.ACTIVE).ToList();
                return usersList;
            }
        }

        public login_verification? getloginVerification(string key)
        {
            using (var db = dbContext)
            {
                var loginVerification = db.login_verifications.Where(x => x.id == key).FirstOrDefault();
                return loginVerification;
            }
        }

        public List<IdNameModel> getUserCompanies(string email)
        {
            using (var db = dbContext)
            {
                db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                string sql = @"SELECT c.id id, c.name
                        FROM users u INNER JOIN companies c ON u.company_id=c.id
                        WHERE u.email='" + email + "'" +
                        @"AND  u.activate_status_id = " + (int)UserActivateStatus.ACTIVE;

                var userCompaniesList = db.idNameModelDB.FromSqlRaw(sql).ToList();
                return userCompaniesList;
            }
        }

        public List<user> getUsers(string email, int? companyId)
        {
            using (var db = dbContext)
            {
                if (companyId != 0)
                {
                    return db.users.Where(x => x.email == email && x.company_id == companyId && x.activate_status_id == (int)UserActivateStatus.ACTIVE).ToList();
                }

                return db.users.Where(x => x.email == email && x.activate_status_id == (int)UserActivateStatus.ACTIVE).ToList();
            }
        }

        public company AddNewCompany(string companyName, string? companyDescr, CompanyActivateStatus status)
        {
            using (var db = dbContext)
            {
                var company = new company
                {
                    name = companyName,
                    descr = companyDescr,
                    activate_status_id = (int)status,
                };

                db.companies.Add(company);
                db.SaveChanges();
                return company;
            }
        }

        public user AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActivateStatus status, UsersRole role, string log)
        {
            using (var db = dbContext)
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

                db.users.Add(user);
                db.SaveChanges();
                return user;
            }
        }

        public company updateCompany(company _company)
        {
            using (var db = dbContext)
            {
                var result = db.companies.Update(_company);
                db.SaveChanges();
                return _company;
            }
        }

        public void addUserPasswordReset(string key, user user)
        {
            using (var db = dbContext)
            {
                FormattableString sql = $@"DELETE FROM password_reset WHERE date_created<=DATE_SUB(NOW(), INTERVAL 1 DAY)";
                int rowsUpdated = db.Database.ExecuteSqlRaw(sql.ToString());

                var pr = new login_verification
                {
                    email = user.email,
                    user_id = user.id,
                    id = key,
                };

                db.login_verifications.Add(pr);
                db.SaveChanges();
            }
        }
    }
}
