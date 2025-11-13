using DataModelsLibrary.Models;

namespace ModuleGeneratorLibrary
{
    public interface IModuleGeneratorService
    {
        Task<ModuleGenerateResponseModel> GenerateModule(ModuleGenerateRequestModel data);
    }
}
