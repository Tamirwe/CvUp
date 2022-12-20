using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
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

        public List<user> getUsersByEmail(string email)
        {
            var usersList = dbContext.users.Where(x => x.email == email).ToList();
            return usersList;
        }

        public registeration_key? getRegistrationKey(string key)
        {
            return dbContext.registeration_keys.Where(x => x.id == key).FirstOrDefault();
        }

        public void removeRegistrationKey(registeration_key rkey)
        {
            var count = dbContext.registeration_keys.Remove(rkey);
        }

        public List<IdNameModel> getUserCompanies(string email)
        {
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            string sql = @"SELECT c.id id, c.name
                        FROM users u INNER JOIN companies c ON u.company_id=c.id
                        WHERE u.email='" + email + "'" +
                    @"AND  u.activate_status_id = " + (int)UserActivateStatus.ACTIVE;

            var userCompaniesList = dbContext.idNameModelDB.FromSqlRaw(sql).ToList();
            return userCompaniesList;
        }

        public user? getUser(int userId)
        {
            return dbContext.users.Where(x => x.id == userId).FirstOrDefault();
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

        public user AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActivateStatus status, UserPermission permission, string log)
        {
            var user = new user
            {
                company_id = companyId,
                email = email,
                passwaord = password,
                first_name = firstName,
                last_name = lastName,
                activate_status_id = (int)status,
                permission_type_id = (int)permission,
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
            FormattableString sql = $@"DELETE FROM registeration_key WHERE date_created<=DATE_SUB(NOW(), INTERVAL 1 DAY)";
            int rowsUpdated = dbContext.Database.ExecuteSqlRaw(sql.ToString());

            var pr = new registeration_key
            {
                email = user.email,
                user_id = user.id,
                id = key,
            };

            dbContext.registeration_keys.Add(pr);
            dbContext.SaveChanges();
        }

        public void activateUser(user user)
        {
            user.activate_status_id = (int)UserActivateStatus.ACTIVE;
            var result = dbContext.users.Update(user);
            dbContext.SaveChanges();
        }

        public void SaveRefreshToken(string refreshToken, user authenticateUser)
        {
            authenticateUser.refresh_token = refreshToken;
            authenticateUser.refresh_token_expiry = DateTime.Now;
            var result = dbContext.users.Update(authenticateUser);
            dbContext.SaveChanges();
        }

        public void RevokeUserToken(int userId)
        {
            user?  user = dbContext.users.Where(x => x.id == userId).FirstOrDefault();

            if (user != null)
            {
                user.refresh_token = null;
                user.refresh_token_expiry = null;
                var result = dbContext.users.Update(user);
                dbContext.SaveChanges();
            }
        }

        public void UpdateUser(user user)
        {
            var result = dbContext.users.Update(user);
            dbContext.SaveChanges();
        }

        public user AddInterviewer(InterviewerModel data,int companyId)
        {
            var user = new user
            {
                company_id = companyId,
                email = data.email,
                first_name = data.firstName,
                last_name = data.lastName,
                activate_status_id = (int)UserActivateStatus.REGISTRATION_NOT_COMPLETED,
                permission_type_id = (int)UserPermission.User,
            };

            dbContext.users.Add(user);
            dbContext.SaveChanges();
            return user;

        }
        public user? UpdateInterviewer(InterviewerModel data, int companyId)
        {
            user? user = dbContext.users.Where(x => x.id == data.id && x.company_id == companyId).FirstOrDefault();

            if (user != null)
            {
                user.email = data.email;
                user.first_name = data.firstName;
                user.last_name = data.lastName;
                user.permission_type_id = (int)data.permissionType;

                dbContext.SaveChanges();
            }

            return user;
        }

        public List<InterviewerModel> GetInterviewersList(int companyId)
        {
            var query = from u in dbContext.users
                        where u.company_id == companyId
                        orderby u.first_name, u.last_name
                        select new InterviewerModel
                        {
                            id = u.id,
                            firstName = u.first_name,
                            lastName = u.last_name,
                            email = u.email,
                            permissionType = (UserPermission)u.permission_type_id
                        };

            return query.ToList();

        }

        public void DeleteInterviewer(int companyId, int id)
        {
            var usr = (from u in dbContext.users
                       where u.id == id && u.company_id == companyId
                       select u).FirstOrDefault();

            if (usr != null)
            {
                var result = dbContext.users.Remove(usr);
                dbContext.SaveChanges();
            }
        }

    }
}
