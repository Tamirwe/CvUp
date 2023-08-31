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
                    first_name = data.firstName,
                    last_name = data.lastName,
                    email = data.email,
                    phone= data.phone,
                    role=data.role,
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
                    customer_id= data.customerId,
                    company_id = companyId,
                    first_name = data.firstName,
                    last_name = data.lastName,
                    email = data.email,
                    phone= data.phone,
                    role= data.role,
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
                            join r in dbContext.customers on c.customer_id equals r.id
                            where c.company_id == companyId
                            orderby c.first_name, c.last_name
                            select new ContactModel
                            {
                                id = c.id,
                                firstName = c.first_name,
                                lastName = c.last_name,
                                email = c.email,
                                phone = c.phone,
                                customerId = c.customer_id,
                                customerName = r.name,
                                role = c.role,
                            };

                return await query.ToListAsync();
            }
        }

        public async Task<customer> AddCustomer(IdNameModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dep = new customer
                {
                    name = data.name,
                    company_id = companyId
                };

                var result = dbContext.customers.Add(dep);
                await dbContext.SaveChangesAsync();
                return result.Entity;
            }
        }

        public async Task<customer?> UpdateCustomer(IdNameModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                customer dep = new customer { id = data.id, name = data.name, company_id = companyId };
                var result = dbContext.customers.Update(dep);
                await dbContext.SaveChangesAsync();
                return result.Entity;
            }
        }

        public async Task<List<IdNameModel>> GetCustomersList(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from dep in dbContext.customers
                            where dep.company_id == companyId
                            orderby dep.name
                            select new IdNameModel
                            {
                                id = dep.id,
                                name = dep.name,
                            };

                return await query.ToListAsync();
            }
        }

        public async Task DeleteCustomer(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dep = await (from d in dbContext.customers
                                 where d.id == id && d.company_id == companyId
                                 select d).FirstOrDefaultAsync();

                if (dep != null)
                {
                    var result = dbContext.customers.Remove(dep);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
