using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataModelsLibrary.Queries
{
    public class ContractTemplatesQueries : IContractTemplatesQueries
    {
        public async Task<List<ContractTemplateModel>> GetContractTemplates()
        {
            using var dbContext = new cvupdbContext();

            return await dbContext.contract_templates
                .AsNoTracking()
                .OrderByDescending(x => x.date_created)
                .Select(x => new ContractTemplateModel
                {
                    id = x.id,
                    name = x.name,
                    descr = x.descr,
                    dateCreated = x.date_created,
                })
                .ToListAsync();
        }

        public async Task<ContractTemplateModel> AddContractTemplate(string name, string? descr)
        {
            using var dbContext = new cvupdbContext();

            var row = new contract_template { name = name, descr = descr };

            dbContext.contract_templates.Add(row);
            await dbContext.SaveChangesAsync();

            return new ContractTemplateModel
            {
                id = row.id,
                name = row.name,
                descr = row.descr,
                dateCreated = row.date_created,
            };
        }

        public async Task<ContractTemplateModel?> GetContractTemplate(int id)
        {
            using var dbContext = new cvupdbContext();

            return await dbContext.contract_templates
                .AsNoTracking()
                .Where(x => x.id == id)
                .Select(x => new ContractTemplateModel
                {
                    id = x.id,
                    name = x.name,
                    descr = x.descr,
                    dateCreated = x.date_created,
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddContractSend(SendContractModel contract, ContractTemplateModel template)
        {
            using var dbContext = new cvupdbContext();

            dbContext.contract_sends.Add(new contract_send
            {
                contract_template_name = template.name,
                contract_template_descr = template.descr,
                company_name = contract.companyName,
                contact_name = contract.contactName,
                email_address = contract.emailAddress,
                email_subject = contract.emailSubject,
                email_body = contract.emailBody,
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<ContractSendModel>> GetContractSends()
        {
            using var dbContext = new cvupdbContext();

            return await dbContext.contract_sends
                .AsNoTracking()
                .OrderByDescending(x => x.date_sent)
                .Select(x => new ContractSendModel
                {
                    id = x.id,
                    contractTemplateName = x.contract_template_name,
                    contractTemplateDescr = x.contract_template_descr,
                    companyName = x.company_name,
                    contactName = x.contact_name,
                    emailAddress = x.email_address,
                    emailSubject = x.email_subject,
                    emailBody = x.email_body,
                    dateSent = x.date_sent,
                })
                .ToListAsync();
        }

        public async Task DeleteContractTemplate(int id)
        {
            using var dbContext = new cvupdbContext();

            var row = await dbContext.contract_templates.FindAsync(id);

            if (row != null)
            {
                dbContext.contract_templates.Remove(row);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
