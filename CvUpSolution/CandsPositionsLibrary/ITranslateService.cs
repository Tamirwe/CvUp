using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandsPositionsLibrary
{
    public interface ITranslateService
    {
        Task<string> SingleLine(string? text, string? language = "en", CancellationToken cancellationToken = default);
        Task<List<string>> MultiLines(List<string>? textList, string? language = "en", CancellationToken cancellationToken = default);
    }
}
