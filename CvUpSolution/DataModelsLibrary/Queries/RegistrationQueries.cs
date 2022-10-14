using Database.models;
using DataModelsLibrary.Enums;

namespace DataModelsLibrary.Queries
{
    public class RegistrationQueries : IRegistrationQueries
    {
        private cvup00001Context dbContext;

        public RegistrationQueries()
        {
            dbContext = new cvup00001Context();
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
                log_info=log
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
    }
}
