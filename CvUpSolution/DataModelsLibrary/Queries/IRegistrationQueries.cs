using Database.models;
using DataModelsLibrary.Enums;

namespace DataModelsLibrary.Queries
{
    public interface IRegistrationQueries
    {
        public company AddNewCompany(string companyName, string? companyDescr, CompanyActivateStatus status);
        user AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActivateStatus status, UsersRole role,string log);
        company updateCompany(company newCompany);
    }
}