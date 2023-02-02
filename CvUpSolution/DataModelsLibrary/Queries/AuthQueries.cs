using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{

    public class AuthQueries : IAuthQueries
    {
        public AuthQueries()
        {
        }

        public List<user> GetUsersByEmail(string email)
        {
            using (var dbContext = new cvup00001Context())
            {
                var usersList = dbContext.users.Where(x => x.email == email).ToList();
                return usersList;
            }
        }

        public registeration_key? GetRegistrationKey(string key)
        {
            using (var dbContext = new cvup00001Context())
            {
                return dbContext.registeration_keys.Where(x => x.id == key).FirstOrDefault();
            }
        }

        public void RemoveRegistrationKey(registeration_key rkey)
        {
            using (var dbContext = new cvup00001Context())
            {
                var count = dbContext.registeration_keys.Remove(rkey);
            }
        }

        public List<IdNameModel> GetUserCompanies(string email)
        {
            using (var dbContext = new cvup00001Context())
            {
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                string sql = @"SELECT c.id id, c.name
                        FROM users u INNER JOIN companies c ON u.company_id=c.id
                        WHERE u.email='" + email + "'" +
                        @"AND  u.active_status = '" + UserActiveStatus.Active.ToString() + "'";

                var userCompaniesList = dbContext.idNameModelDB.FromSqlRaw(sql).ToList();
                return userCompaniesList;
            }
        }

        public user? GetUser(int userId)
        {
            using (var dbContext = new cvup00001Context())
            {
                return dbContext.users.Where(x => x.id == userId).FirstOrDefault();
            }
        }

        public company? GetCompany(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                return dbContext.companies.Where(x => x.id == companyId).FirstOrDefault();
            }
        }

        public List<user> GetUsers(string email, int? companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                if (companyId != 0)
                {
                    return dbContext.users.Where(x => x.email == email && x.company_id == companyId && x.active_status == UserActiveStatus.Active.ToString()).ToList();
                }

                return dbContext.users.Where(x => x.email == email && x.active_status == UserActiveStatus.Active.ToString()).ToList();
            }
        }

        public company AddNewCompany(string companyName, string? companyDescr, CompanyActiveStatus status)
        {
            using (var dbContext = new cvup00001Context())
            {
                var company = new company
                {
                    name = companyName,
                    descr = companyDescr,
                    active_status = status.ToString(),
                };

                dbContext.companies.Add(company);
                dbContext.SaveChanges();
                return company;
            }
        }

        public user AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActiveStatus status, UserPermission permission, string log)
        {
            using (var dbContext = new cvup00001Context())
            {
                var user = new user
                {
                    company_id = companyId,
                    email = email,
                    passwaord = password,
                    first_name = firstName,
                    last_name = lastName,
                    active_status = status.ToString(),
                    permission_type = permission.ToString(),
                    log_info = log
                };

                dbContext.users.Add(user);
                dbContext.SaveChanges();
                return user;
            }
        }

        public company UpdateCompany(company _company)
        {
            using (var dbContext = new cvup00001Context())
            {
                var result = dbContext.companies.Update(_company);
                dbContext.SaveChanges();
                return _company;
            }
        }

        public void AddUserPasswordReset(string key, user user)
        {
            using (var dbContext = new cvup00001Context())
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
        }

        public void ActivateUser(user user)
        {
            using (var dbContext = new cvup00001Context())
            {
                user.active_status = UserActiveStatus.Active.ToString();
                var result = dbContext.users.Update(user);
                dbContext.SaveChanges();
            }
        }

        public void SaveRefreshToken(string refreshToken, user authenticateUser)
        {
            using (var dbContext = new cvup00001Context())
            {
                authenticateUser.refresh_token = refreshToken;
                authenticateUser.refresh_token_expiry = DateTime.Now;
                var result = dbContext.users.Update(authenticateUser);
                dbContext.SaveChanges();
            }
        }

        public void RevokeUserToken(int userId)
        {
            using (var dbContext = new cvup00001Context())
            {
                user? user = dbContext.users.Where(x => x.id == userId).FirstOrDefault();

                if (user != null)
                {
                    user.refresh_token = null;
                    user.refresh_token_expiry = null;
                    var result = dbContext.users.Update(user);
                    dbContext.SaveChanges();
                }
            }
        }

        public void UpdateUser(user user)
        {
            using (var dbContext = new cvup00001Context())
            {
                var result = dbContext.users.Update(user);
                dbContext.SaveChanges();
            }
        }

        public void AddInterviewer(InterviewerModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var user = new user
                {
                    company_id = companyId,
                    email = data.email,
                    first_name = data.firstName,
                    last_name = data.lastName,
                    active_status = UserActiveStatus.Waite_Complete_Registration.ToString(),
                    permission_type = UserPermission.User.ToString(),
                };

                dbContext.users.Add(user);
                dbContext.SaveChanges();
            }
        }

        public void UpdateInterviewer(InterviewerModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                user? user = dbContext.users.Where(x => x.id == data.id && x.company_id == companyId).FirstOrDefault();

                if (user != null)
                {
                    user.email = data.email;
                    user.first_name = data.firstName;
                    user.last_name = data.lastName;
                    user.permission_type = data.permissionType.ToString();

                    var result = dbContext.users.Update(user);
                    dbContext.SaveChanges();
                }
            }
        }

        public List<InterviewerModel> GetInterviewersList(int companyId)
        {
            using (var dbContext = new cvup00001Context())
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
                                permissionType = Enum.Parse<UserPermission>(u.permission_type)
                            };

                return query.ToList();

            }
        }

        public void DeleteInterviewer(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
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

        public company? GetCompanyByKeyEmail(string uniqueRandomKey)
        {
            using (var dbContext = new cvup00001Context())
            {
                return dbContext.companies.Where(x=>x.key_email== uniqueRandomKey).FirstOrDefault();

            }
        }

        public void AddCompanySatrterData(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {

                var parser = new company_parser
                {
                    company_id = companyId,
                    parser_id=1
                };

                dbContext.company_parsers.Add(parser);

                var stages = new position_candidate_stage
                {
                    id=1,
                    company_id = companyId,
                    name = "Attached"
                };

                dbContext.position_candidate_stages.Add(stages);

                dbContext.SaveChanges();

            }
        }

    }
}
