using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.EmbeddingAndStore
{
    public interface IStoreService
    {
        Task EnsureCollectionAsync();
        Task UpsertAsync(Guid id, EmbedCvDataModel cv);
        Task UpsertBatchAsync(List<EmbedCvDataModel> cvs);
    }
}
