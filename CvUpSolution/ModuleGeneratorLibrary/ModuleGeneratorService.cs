using DataModelsLibrary.Models;

namespace ModuleGeneratorLibrary
{
    public class ModuleGeneratorService : IModuleGeneratorService
    {

        public Task<ModuleGenerateResponseModel> GenerateModule(ModuleGenerateRequestModel data)
        {
            ModuleGenerateResponseModel reader = new ModuleGenerateResponseModel();

            return Task.FromResult(reader); ;
        }
    }
}
