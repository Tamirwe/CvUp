using DataModelsLibrary.Models;

namespace ServicesLibrary.RegisterCompanyAndUser
{
    public interface IRegisterCompanyAndUserServise
    {
        void Register(CompanyAndUserRegisetModel data);
    }
}