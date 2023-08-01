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

        public async Task AddInterviewer(InterviewerModel data, int companyId)
        {
            await _authQueries.AddInterviewer(data, companyId);
        }

        public async Task UpdateInterviewer(InterviewerModel data, int companyId)
        {
            await _authQueries.UpdateInterviewer(data, companyId);
        }

        public async Task<List<InterviewerModel>> GetInterviewersList(int companyId)
        {
            List<InterviewerModel> depList = await _authQueries.GetInterviewersList(companyId);
            return depList;
        }

        public async Task<UserModel?> GetUser(int companyId, int userId)
        {
            UserModel? userData = await _authQueries.GetUser(companyId, userId);
            return userData;
        }


    }
}