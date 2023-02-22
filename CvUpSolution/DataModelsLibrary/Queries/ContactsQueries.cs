using Database.models;
using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public class ContactsQueries : IContactsQueries
    {
        public async Task<folder> AddContact(FolderModel data)
        {
            throw new NotImplementedException();
        }

        public async Task Deletecontact(int companyId, int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FolderModel>> GetContacts(int companyId, int id)
        {
            throw new NotImplementedException();
        }
    }
}
