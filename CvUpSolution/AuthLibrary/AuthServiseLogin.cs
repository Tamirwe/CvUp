using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace AuthLibrary
{
    public partial class AuthServise
    {
        public user? Login(UserLoginModel data)
        {
            var users = _authQueries.GetUsersByEmail(data.email);
            bool isVerify = false;

            foreach (var usr in users)
            {
                 isVerify = SecretHasher.Verify(data.password, usr.passwaord ?? "");

                if (isVerify )
                {
                    if( usr.active_status == UserActiveStatus.Active.ToString())
                    {
                        return usr;
                    }

                    return null;
                }
            }

            return null;
        }

        public user? CompleteRegistration(UserLoginModel data)
        {
            registeration_key?  rKey = _authQueries.GetRegistrationKey(data.key);

            if (rKey is not null)
            {
                var user = _authQueries.GetUser(rKey.user_id);

                if (user is not null)
                {
                    bool isVerify = SecretHasher.Verify(data.password, user.passwaord ?? "");

                    if (isVerify)
                    {
                        if (user.active_status != UserActiveStatus.Active.ToString())
                        {
                            _authQueries.ActivateUser(user);
                            var company = _authQueries.GetCompany(user.company_id);

                            if (company is not null && company.active_status == CompanyActiveStatus.Waite_Complete_Registration.ToString())
                            {
                                company.active_status = CompanyActiveStatus.Active.ToString();
                                _authQueries.UpdateCompany(company);
                            }
                        }

                        return user;
                    }
                }
            }

            return null;
        }

        
    }
}
