using Google.Cloud.Translate.V3;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandsPositionsLibrary
{
    public class TranslateService : ITranslateService
    {
        private static readonly string _projectId = "253413272327";

        private static readonly AsyncLazy<TranslationServiceClient> _client = new(() => TranslationServiceClient.CreateAsync());

        public async Task<string> SingleLine(string? text, string? language = "en", CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                TranslationServiceClient client = await _client;

                var response = await client.TranslateTextAsync($"projects/{_projectId}/locations/global", language, new string[] { text }, cancellationToken);

                return response.Translations[0].TranslatedText;
            }

            return "";
        }

        public async Task<List<string>> MultiLines(List<string>? textList, string? language="en", CancellationToken cancellationToken = default)
        {
            TranslationServiceClient client = await _client;
            var response = await client.TranslateTextAsync($"projects/{_projectId}/locations/global", language, textList, cancellationToken);

            return response.Translations.Select(x=>x.TranslatedText).ToList();

            ////var list = new List<string> { "מהנדס תעשייה וניהול", "קצין ציות" };
            //return new List<string> { "vvvv" };

        }
    }
}
