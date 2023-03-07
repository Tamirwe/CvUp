using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public class ContactsQueries : IContactsQueries
    {
        public async Task<contact> AddContact(int companyId, ContactModel data)
        {
            using (var dbContext = new cvup00001Context())
            {
                var cont = new contact
                {
                    customer_id= data.customerId,
                    company_id = companyId,
                    name= data.name,
                    email= data.email,
                    phone= data.phone,
                };

                dbContext.contacts.Add(cont);
                await dbContext.SaveChangesAsync();
                return cont;
            }
        }

        public async Task<contact> UpdateContact(int companyId, ContactModel data)
        {
            using (var dbContext = new cvup00001Context())
            {
                var fdr = new contact
                {
                    id = data.id,
                    name = data.name,
                    email= data.email,
                    phone= data.phone,
                };

                dbContext.contacts.Update(fdr);
                await dbContext.SaveChangesAsync();
                return fdr;
            }
        }


        public async Task Deletecontact(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var cont = await (from c in dbContext.contacts
                                 where c.id == id && c.company_id == companyId
                                 select c).FirstOrDefaultAsync();

                if (cont != null)
                {
                    dbContext.contacts.Remove(cont);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<ContactModel>> GetContacts(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from c in dbContext.contacts
                            where c.company_id == companyId
                            orderby c.name
                            select new ContactModel
                            {
                                id = c.id,
                                name = c.name,
                                email = c.email,
                                phone = c.phone,
                                customerId = c.customer_id
                            };

                return await query.ToListAsync();
            }
        }
    }
}
