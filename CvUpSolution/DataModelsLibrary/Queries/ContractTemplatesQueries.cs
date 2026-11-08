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
