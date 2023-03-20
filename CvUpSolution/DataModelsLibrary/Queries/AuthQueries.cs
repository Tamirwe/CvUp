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

        public async Task<List<user>> GetUsersByEmail(string email)
        {
            using (var dbContext = new cvup00001Context())
            {
                var usersList = await dbContext.users.Where(x => x.email == email).ToListAsync();
                return usersList;
            }
        }

        public async Task<registeration_key?> GetRegistrationKey(string key)
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.registeration_keys.Where(x => x.id == key).FirstOrDefaultAsync();
            }
        }

        public async Task RemoveRegistrationKey(registeration_key rkey)
        {
            using (var dbContext = new cvup00001Context())
            {
                var count = dbContext.registeration_keys.Remove(rkey);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<IdNameModel>> GetUserCompanies(string email)
        {
            using (var dbContext = new cvup00001Context())
            {
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                string sql = @"SELECT c.id id, c.name
                        FROM users u INNER JOIN companies c ON u.company_id=c.id
                        WHERE u.email='" + email + "'" +
                        @"AND  u.active_status = '" + UserActiveStatus.Active.ToString() + "'";

                var userCompaniesList = await dbContext.idNameModelDB.FromSqlRaw(sql).ToListAsync();
                return userCompaniesList;
            }
        }

        public async Task<user?> GetUser(int userId)
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.users.Where(x => x.id == userId).FirstOrDefaultAsync();
            }
        }

        public async Task<company?> GetCompany(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.companies.Where(x => x.id == companyId).FirstOrDefaultAsync();
            }
        }

        public async Task<List<user>> GetUsers(string email, int? companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                if (companyId != 0)
                {
                    return await dbContext.users.Where(x => x.email == email && x.company_id == companyId && x.active_status == UserActiveStatus.Active.ToString()).ToListAsync();
                }

                return await dbContext.users.Where(x => x.email == email && x.active_status == UserActiveStatus.Active.ToString()).ToListAsync();
            }
        }

        public async Task<company> AddNewCompany(string companyName, string? companyDescr, CompanyActiveStatus status)
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
                await dbContext.SaveChangesAsync();
                return company;
            }
        }

        public async Task<user> AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActiveStatus status, UserPermission permission, string log)
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
                await dbContext.SaveChangesAsync();
                return user;
            }
        }

        public async Task<company> UpdateCompany(company _company)
        {
            using (var dbContext = new cvup00001Context())
            {
                var result = dbContext.companies.Update(_company);
                await dbContext.SaveChangesAsync();
                return _company;
            }
        }

        public async Task AddUserPasswordReset(string key, user user)
        {
            using (var dbContext = new cvup00001Context())
            {
                var pr = new registeration_key
                {
                    email = user.email,
                    user_id = user.id,
                    id = key,
                };

                dbContext.registeration_keys.Add(pr);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteOldRegistrationsKeys()
        {
            using (var dbContext = new cvup00001Context())
            {
                string sql = @"DELETE FROM registeration_key WHERE date_created<=DATE_SUB(NOW(), INTERVAL 1 DAY)";
                int rowsUpdated = await dbContext.Database.ExecuteSqlRawAsync(sql);
            }
        }

        public async Task ActivateUser(user user)
        {
            using (var dbContext = new cvup00001Context())
            {
                user.active_status = UserActiveStatus.Active.ToString();
                var result = dbContext.users.Update(user);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task SaveRefreshToken(string refreshToken, user authenticateUser)
        {
            using (var dbContext = new cvup00001Context())
            {
                authenticateUser.refresh_token = refreshToken;
                authenticateUser.refresh_token_expiry = DateTime.Now;
                var result = dbContext.users.Update(authenticateUser);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RevokeUserToken(int userId)
        {
            using (var dbContext = new cvup00001Context())
            {
                user? user = await dbContext.users.Where(x => x.id == userId).FirstOrDefaultAsync();

                if (user != null)
                {
                    user.refresh_token = null;
                    user.refresh_token_expiry = null;
                    var result = dbContext.users.Update(user);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateUser(user user)
        {
            using (var dbContext = new cvup00001Context())
            {
                var result = dbContext.users.Update(user);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task AddInterviewer(InterviewerModel data, int companyId)
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
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateInterviewer(InterviewerModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                user? user = await dbContext.users.Where(x => x.id == data.id && x.company_id == companyId).FirstOrDefaultAsync();

                if (user != null)
                {
                    user.email = data.email;
                    user.first_name = data.firstName;
                    user.last_name = data.lastName;
                    user.permission_type = data.permissionType.ToString();

                    var result = dbContext.users.Update(user);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<InterviewerModel>> GetInterviewersList(int companyId)
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

                return await query.ToListAsync();

            }
        }

        public async Task DeleteInterviewer(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var usr = await (from u in dbContext.users
                           where u.id == id && u.company_id == companyId
                           select u).FirstOrDefaultAsync();

                if (usr != null)
                {
                    var result = dbContext.users.Remove(usr);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<company_cvs_email?> GetCompanyEmail(string newEmail)
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.company_cvs_emails.Where(x => x.email == newEmail).FirstOrDefaultAsync();
            }
        }

        public async Task AddCompanySatrterData(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {

                var parser = new company_parser
                {
                    company_id = companyId,
                    parser_id = 1
                };

                dbContext.company_parsers.Add(parser);

                var stages = new position_candidate_stage
                {
                    id = 1,
                    company_id = companyId,
                    name = "Attached"
                };

                dbContext.position_candidate_stages.Add(stages);

                await dbContext.SaveChangesAsync();

            }
        }

        public async Task AddCompanyCvsEmail(int companyId, string generatedCompanyEmail)
        {
            using (var dbContext = new cvup00001Context())
            {
                var newRec = new company_cvs_email
                {
                    company_id = companyId,
                    email = generatedCompanyEmail,
                };

                dbContext.company_cvs_emails.Add(newRec);
                await dbContext.SaveChangesAsync();
            }
        }


    }
}