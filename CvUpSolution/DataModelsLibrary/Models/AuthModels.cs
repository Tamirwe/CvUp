using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Models
{
   public class CompanyAndUserRegisetModel
    {
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string companyName { get; set; } = string.Empty;
        public string? companyDescr { get; set; }
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;

    }
    public class UserLoginModel
    {
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string key { get; set; } = string.Empty;
        public bool rememberMe { get; set; } = false;

    }

    public class ForgotPasswordModel
    {
        public string email { get; set; } = string.Empty;
        public int? companyId { get; set; } = 0;

    }

    public class TokenModel
    {
        public string token { get; set; } = string.Empty;
        public string refreshToken { get; set; } = string.Empty;

    }
    public class InterviewerModel
    {
        public int id { get; set; } = 0;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }

}
