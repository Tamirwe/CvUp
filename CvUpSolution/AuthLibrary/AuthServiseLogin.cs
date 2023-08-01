using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace AuthLibrary
{
    public partial class AuthServise
    {
        public async Task<user?> Login(UserLoginModel data)
        {
            var users = await _authQueries.GetUsersByEmail(data.email);
            bool isVerify = false;

            foreach (var usr in users)
            {
                isVerify = SecretHasher.Verify(data.password, usr.passwaord ?? "");

                if (isVerify)
                {
                    if (usr.active_status == UserActiveStatus.Active.ToString())
                    {
                        return usr;
                    }

                    return null;
                }
            }

            return null;
        }

        public async Task<user?> CompleteRegistration(UserLoginModel data)
        {
            registeration_key? rKey = await _authQueries.GetRegistrationKey(data.key);

            if (rKey is not null)
            {
                var user = await _authQueries.GetUser(rKey.user_id);

                if (user is not null)
                {
                    if (string.IsNullOrEmpty(user.passwaord))
                    {
                        string hashPassword = SecretHasher.Hash(data.password);
                        user.passwaord = hashPassword;
                    }

                    if (user.active_status != UserActiveStatus.Active.ToString())
                    {
                        user.active_status = UserActiveStatus.Active.ToString();
                        await _authQueries.UpdateUser(user);
                        var company = await _authQueries.GetCompany(user.company_id);

                        if (company is not null && company.active_status == CompanyActiveStatus.Waite_Complete_Registration.ToString())
                        {
                            company.active_status = CompanyActiveStatus.Active.ToString();
                            await _authQueries.UpdateCompany(company);
                        }
                    }

                    return user;

                }
            }

            return null;
        }
    }
}