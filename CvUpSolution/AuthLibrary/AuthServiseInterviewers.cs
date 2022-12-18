using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthLibrary
{
    public partial class AuthServise
    {

        public user AddInterviewer(InterviewerModel data, int companyId)
        {
            user newUser = _authQueries.AddInterviewer(data, companyId);
            return newUser;
        }

        public user UpdateInterviewer(InterviewerModel data, int companyId)
        {
            user updatedUser = _authQueries.UpdateInterviewer(data, companyId);
            return updatedUser;
        }

        public List<IdNameModel> GetInterviewers(int companyId)
        {
            List<IdNameModel> depList = _authQueries.GetInterviewers(companyId);
            return depList;
        }

        public InterviewerModel? DeleteInterviewer(int companyId, int id)
        {
            InterviewerModel result = _authQueries.DeleteInterviewer(companyId, id);
            return result;
        }

    }
}
