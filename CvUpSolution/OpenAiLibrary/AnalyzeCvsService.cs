using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace OpenAiLibrary
{
    public class AnalyzeCvsService : IAnalyzeCvsService
    {
        asdasdasdasdasdasd

        private ICandsCvsQueries _candsCvsQueries;

        public AnalyzeCvsService(ICandsCvsQueries candsCvsQueries)
        {
            _candsCvsQueries = candsCvsQueries;
        }

        public async Task<List<CandCvTxtModel>> GetCandsLastCvText(int companyId = 154, int candidateId = 0)
        {


            string jsonString =  await File.ReadAllTextAsync("israeliCities.json");
            List<IsraeliCities> citiesRegion = JsonSerializer.Deserialize<List<IsraeliCities>>(jsonString)!;

            List<CandCvTxtModel> cvPropsToIndexList = await _candsCvsQueries.GetCandsLastCvText(companyId);
            return cvPropsToIndexList;
        }


    }
}

public class IsraeliCities
{
    public required string city { get; set; }
    public required string region { get; set; }
}