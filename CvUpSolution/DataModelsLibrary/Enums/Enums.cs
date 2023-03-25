using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Enums
{
  
    public enum CompanyActiveStatus
    {
        Active,
        Waite_Complete_Registration,
        Not_Active ,
    }

    public enum UserActiveStatus
    {
        Active,
        Waite_Complete_Registration,
        Not_Active,
    }

    public enum UserAuthStatus
    {
        Authenticated = 1,
        not_registered = 2,
        more_then_one_company_per_email = 3,
    }
   
    public enum UserPermission
    {
        Admin,
        User,
    }

    public enum EmailType
    {
        Registration_Approved,
        Confirm_Registration
    }
    public enum Lung
    {
        HE,
        EN,
    }
    public enum ParserValueType
    {
        Name,
        Position,
        Address,
        CompanyType
    }
}
