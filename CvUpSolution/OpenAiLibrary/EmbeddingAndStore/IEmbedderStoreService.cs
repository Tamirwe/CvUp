using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.EmbeddingAndStore
{
    public interface IEmbedderStoreService
    {
        Task EmbedAnalyzedCvs();
    }
}
