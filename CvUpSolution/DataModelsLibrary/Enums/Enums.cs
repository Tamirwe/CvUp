using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Enums
{
    public enum CompanyActivateStatus
    {
        ACTIVE = 10,
        WAITE_FOR_FIRST_USER_TO_COMPLETE_REGISTRATION = 20,
        NOT_ACTIVE = 30,
    }

    public enum UserActivateStatus
    {
        ACTIVE = 10,
        REGISTRATION_NOT_COMPLETED = 20,
        NOT_ACTIVE = 30,
    }

    public enum UserLoginStatus
    {
        Authenticated = 1,
        not_registered = 2,
        more_then_one_company_per_email = 3,
    }
    public enum UsersRole
    {
        Admin = 10,
        User = 20,
    }
    public enum EmailType
    {
        APPROVE_REGISTRATION = 1,
        REGISTRATION_CONFIRMATION = 2,
        CV_ASSAINGED_TO_POSITION = 3
    }
    public enum lung
    {
        HE = 1,
        EN_US = 2,
    }
}
