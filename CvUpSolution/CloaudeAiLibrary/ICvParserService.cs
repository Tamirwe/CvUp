using CloaudeAiLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloaudeAiLibrary
{
    public interface ICvParserService
    {
        Task<ParsedCvModel> ParseAsync(string rawCvText);
    }
}
