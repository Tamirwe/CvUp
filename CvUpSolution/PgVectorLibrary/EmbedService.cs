using CvAnalyzeEmbedOpenAiLibrary;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace PgVectorLibrary
{
    public class EmbedService : IEmbedService
    {
        private readonly ICandsCvsQueries _candsCvsQueries;
        private readonly IEmbedCvOpenAi _embedCvOpenAi;
        private readonly int _companyId;

        public EmbedService(ICandsCvsQueries candsCvsQueries, IEmbedCvOpenAi embedCvOpenAi, int companyId = 154)
        {

            _candsCvsQueries = candsCvsQueries;
            _embedCvOpenAi = embedCvOpenAi;
            _companyId = companyId;
        }

        public async Task EmbedAnalyzeCvs()
        {
            List<AnalyzedCvsForEmbeedingModel> analyzedCvsForEmbeedingList = await _candsCvsQueries.GetAnalyzedCvsForEmbeeding();

        }
    }
}
