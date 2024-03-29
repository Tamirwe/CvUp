﻿using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IAuthQueries
    {
        Task<List<user>> GetUsersByEmail(string email);
        Task<List<user>> GetUsersByCompanyEmail(string email, int companyId);
        Task<List<IdNameModel>> GetUserCompanies(string email);
        Task<user?> GetUser(int userId);
        Task<user?> GetUser(int companyId, string email);
        Task<List<user>> GetUsers(string email, int? companyId);
        Task<UserModel?> GetUser(int companyId, int userId);
        Task<List<UserModel>> GetUsers(int companyId);
        Task<company?> GetCompany(int companyId);
        Task<company> AddCompany(string companyName, string? companyDescr, CompanyActiveStatus status);
        Task<user> AddUser(int companyId, string email, string? password, string firstName, string lastName, string? firstNameEn, string? lastNameEn, UserActiveStatus status, UserPermission permission, string log);
        Task<company> UpdateCompany(company newCompany);
        Task AddRegistrationKey(string key, user user);
        Task DeleteOldRegistrationsKeys();
        Task<registeration_key?> GetRegistrationKey(string key);
        Task RevokeUser(int companyId, int userId);
        Task RemoveRegistrationKey(registeration_key rkey);
        Task UpdateUser(user user);
        Task AddInterviewer(InterviewerModel data, int companyId);
        Task UpdateInterviewer(InterviewerModel data, int companyId);
        Task<List<InterviewerModel>> GetInterviewersList(int companyId);
        Task<company_cvs_email?> GetCompanyEmail(string newEmail);
        Task AddCompanySatrterData(int companyId);
        Task AddCompanyCvsEmail(int companyId, string generatedCompanyEmail);
        Task UpdateCompanyUser(UserModel data, int companyId);
        Task DeleteUser(int companyId, int id);
        Task<users_refresh_token?> GetUserRefreshTokens(int companyId, int userId, string refreshToken);
        Task UPdateRefreshToken(users_refresh_token newRefreshToken);
        Task DeleteExpiredTokens();
        Task AddUserRefreshToken(int companyId, int userId, string newRefreshToken, int refreshTokenHoursExpiration);
    }
}