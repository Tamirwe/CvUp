using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataModelsLibrary.Queries
{
    public class CustomerContractsQueries : ICustomerContractsQueries
    {
        // contract_templates has no company_id -- the rows are shared, and the
        // .docx behind each one lives in the company's own folder. Whether a
        // template is usable is a filesystem question, answered in the service.
        public async Task<List<ContractTemplateModel>> GetContractTemplates()
        {
            using (var dbContext = new cvupdbContext())
            {
                var query = from ct in dbContext.contract_templates
                            orderby ct.name
                            select new ContractTemplateModel
                            {
                                id = ct.id,
                                name = ct.name,
                                descr = ct.descr,
                                dateCreated = ct.date_created,
                                dateUpdated = ct.date_updated,
                            };

                return await query.ToListAsync();
            }
        }

        public async Task<ContractTemplateModel?> GetContractTemplate(int id)
        {
            using (var dbContext = new cvupdbContext())
            {
                var query = from ct in dbContext.contract_templates
                            where ct.id == id
                            select new ContractTemplateModel
                            {
                                id = ct.id,
                                name = ct.name,
                                descr = ct.descr,
                                dateCreated = ct.date_created,
                                dateUpdated = ct.date_updated,
                            };

                return await query.FirstOrDefaultAsync();
            }
        }

        public async Task<contract_template> AddContractTemplate(ContractTemplateModel data)
        {
            using (var dbContext = new cvupdbContext())
            {
                var rec = new contract_template
                {
                    name = data.name,
                    descr = data.descr,
                    date_created = DateTime.Now,
                    date_updated = DateTime.Now,
                };

                dbContext.contract_templates.Add(rec);
                await dbContext.SaveChangesAsync();
                return rec;
            }
        }

        public async Task<contract_template?> UpdateContractTemplate(ContractTemplateModel data)
        {
            using (var dbContext = new cvupdbContext())
            {
                contract_template? rec = await dbContext.contract_templates
                    .Where(x => x.id == data.id).FirstOrDefaultAsync();

                if (rec == null)
                {
                    return null;
                }

                rec.name = data.name;
                rec.descr = data.descr;
                rec.date_updated = DateTime.Now;

                dbContext.contract_templates.Update(rec);
                await dbContext.SaveChangesAsync();
                return rec;
            }
        }

        public async Task DeleteContractTemplate(int id)
        {
            using (var dbContext = new cvupdbContext())
            {
                contract_template? rec = await dbContext.contract_templates
                    .Where(x => x.id == id).FirstOrDefaultAsync();

                if (rec != null)
                {
                    dbContext.contract_templates.Remove(rec);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<customer_contract> AddCustomerContract(SendContractModel data, DateTime dateSent)
        {
            using (var dbContext = new cvupdbContext())
            {
                var rec = new customer_contract
                {
                    customer_id = data.customerId,
                    contact_id = data.contactId,
                    contract_template_id = data.contractTemplateId,
                    descr = data.descr,
                    email_subject = data.subject,
                    email_body = data.body,
                    date_sent = dateSent,
                    date_created = DateTime.Now,
                };

                dbContext.customer_contracts.Add(rec);
                await dbContext.SaveChangesAsync();
                return rec;
            }
        }

        public async Task<CustomerContractModel?> GetLatestCustomerContract(int companyId, int customerId)
        {
            using (var dbContext = new cvupdbContext())
            {
                var query = from cc in ContractsQuery(dbContext, companyId)
                            where cc.customerId == customerId
                            select cc;

                return await query.FirstOrDefaultAsync();
            }
        }

        public async Task<List<CustomerContractModel>> GetLatestContracts(int companyId, int take)
        {
            using (var dbContext = new cvupdbContext())
            {
                return await ContractsQuery(dbContext, companyId).Take(take).ToListAsync();
            }
        }

        public async Task<ContractMergeDataModel?> GetMergeData(int companyId, int customerId, int contactId)
        {
            using (var dbContext = new cvupdbContext())
            {
                var query = from cu in dbContext.customers
                            join co in dbContext.contacts on cu.id equals co.customer_id
                            where cu.company_id == companyId
                                  && cu.id == customerId
                                  && co.id == contactId
                            select new ContractMergeDataModel
                            {
                                customerName = cu.name,
                                customerAddress = cu.address,
                                customerDescr = cu.descr,
                                contactName = (co.first_name + " " + (co.last_name ?? string.Empty)).Trim(),
                                contactEmail = co.email,
                            };

                return await query.FirstOrDefaultAsync();
            }
        }

        // Newest first. customer_contracts has no company_id of its own, so the
        // company comes from the customer the contract belongs to.
        private IQueryable<CustomerContractModel> ContractsQuery(cvupdbContext dbContext, int companyId)
        {
            return from cc in dbContext.customer_contracts
                   join cu in dbContext.customers on cc.customer_id equals cu.id
                   join co in dbContext.contacts on cc.contact_id equals co.id into contacts
                   from co in contacts.DefaultIfEmpty()
                   join ct in dbContext.contract_templates on cc.contract_template_id equals ct.id into templates
                   from ct in templates.DefaultIfEmpty()
                   where cu.company_id == companyId
                   orderby cc.date_sent descending, cc.id descending
                   select new CustomerContractModel
                   {
                       id = cc.id,
                       customerId = cc.customer_id,
                       customerName = cu.name,
                       contactId = cc.contact_id,
                       contactName = co == null
                           ? string.Empty
                           : (co.first_name + " " + (co.last_name ?? string.Empty)).Trim(),
                       contactEmail = co == null ? string.Empty : co.email,
                       contractTemplateId = cc.contract_template_id,
                       contractTemplateName = ct == null ? string.Empty : ct.name,
                       descr = cc.descr,
                       emailSubject = cc.email_subject,
                       emailBody = cc.email_body,
                       dateSent = cc.date_sent,
                       dateCreated = cc.date_created,
                   };
        }
    }
}
