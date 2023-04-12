﻿using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IContactsQueries
    {
        Task<contact> AddContact(int companyId, ContactModel data);
        Task Deletecontact(int companyId, int id);
        Task<List<ContactModel>> GetContacts(int companyId);
        Task<contact> UpdateContact(int companyId, ContactModel data);
        public Task<customer> AddCustomer(IdNameModel data, int companyId);
        public Task<customer?> UpdateCustomer(IdNameModel data, int companyId);
        public Task<List<IdNameModel>> GetCustomersList(int companyId);
        public Task DeleteCustomer(int companyId, int id);
    }
}