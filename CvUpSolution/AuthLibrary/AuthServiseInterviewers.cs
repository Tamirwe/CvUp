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

        public void AddInterviewer(InterviewerModel data, int companyId)
        {
            _authQueries.AddInterviewer(data, companyId);
        }

        public void UpdateInterviewer(InterviewerModel data, int companyId)
        {
            _authQueries.UpdateInterviewer(data, companyId);
        }

        public List<InterviewerModel> GetInterviewersList(int companyId)
        {
            List<InterviewerModel> depList = _authQueries.GetInterviewersList(companyId);
            return depList;
        }

        public void DeleteInterviewer(int companyId, int id)
        {
            _authQueries.DeleteInterviewer(companyId, id);
        }

    }
}
