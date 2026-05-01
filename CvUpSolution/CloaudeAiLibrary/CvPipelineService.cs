using CloaudeAiLibrary.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloaudeAiLibrary
{
    public class CvPipelineService
    {
        private readonly CvParserService _parser;
        private readonly CvIndexingService _indexer;
        private readonly ILogger<CvPipelineService> _logger;

        public CvPipelineService(
            CvParserService parser,
            CvIndexingService indexer,
            ILogger<CvPipelineService> logger)
        {
            _parser = parser;
            _indexer = indexer;
            _logger = logger;
        }

        public async Task ProcessCvsAsync(IEnumerable<string> rawCvTexts)
        {
            var semaphore = new SemaphoreSlim(5); // max 5 concurrent Claude calls

            var tasks = rawCvTexts.Select(async rawText =>
            {
                await semaphore.WaitAsync();
                try
                {
                    ParsedCvModel parsed = await _parser.ParseAsync(rawText);
                    await _indexer.IndexCvAsync(parsed);

                    _logger.LogInformation(
                        "✅ Indexed: {Name} | {Profession} | Skills: {Skills}",
                        parsed.FullName,
                        parsed.Profession,
                        string.Join(", ", parsed.Skills)
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to process CV");
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
