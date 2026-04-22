using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.EmbeddingQdrant
{
    public interface IEmbedderStoreService
    {
        Task EmbedAnalyzedCvs(string apiKey, int companyId = 154);
    }
}
