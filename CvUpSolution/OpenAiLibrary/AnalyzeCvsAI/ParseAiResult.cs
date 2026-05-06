using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAiLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAiLibrary.AnalyzeCvsAI
{

    internal static class ParseAiResult
    {
        public static AnalyzedCvModel ParseResult(string json)
        {
            // Strip markdown fences if the model returns them anyway
            json = json
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            // Extract the first { ... } block in case of extra text
            int start = json.IndexOf('{');
            int end = json.LastIndexOf('}');

            if (start >= 0 && end > start)
                json = json[start..(end + 1)];

            json = FixUnbalancedQuotes(json);

            try
            {

                var obj = JObject.Parse(json);

                return new AnalyzedCvModel
                {
                    Name = obj.Value<string>("name"),
                    Email = obj.Value<string>("email"),
                    Phone = obj.Value<string>("phone"),
                    CityHe = obj.Value<string>("city_he"),
                    Languages = obj.Value<string>("languages"),
                    WorkExperience = obj["work_experience"]?.ToObject<List<string>>() ?? [],
                    ProfessionWords =obj["profession_words"]?.ToObject<List<string>>() ?? [], 
                    ProfessionSkills = obj["profession_skills"]?.ToObject<List<string>>() ?? [],
                    Seniority = obj.Value<string>("seniority"),
                    Education = obj["education_he"]?.ToObject<List<string>>() ?? [],
                    Skills = obj["skills"]?.ToObject<List<string>>() ?? [],
                    MilitaryService = obj.Value<string>("military_service_he"),
                    SummaryEn = obj.Value<string>("summary_en") ?? "",
                    SummaryHe = obj.Value<string>("summary_he") ?? "",
                    YearsExperience = myParseInt(obj.Value<string>("years_experience")),
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [warn] JSON parse failed: {ex.Message}");
                Console.WriteLine($"  [raw]  {json[..Math.Min(200, json.Length)]}");
                throw ex;

            }
        }

        private static int? myParseInt(string? val)
        {
            if (int.TryParse(val, out int result))
            {
                return result;
            }
            return null;
        }

        public static string FixUnbalancedQuotes(string json)
        {
            bool inString = false;
            var result = new System.Text.StringBuilder();

            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];

                if (c == '"' && (i == 0 || json[i - 1] != '\\'))
                {
                    inString = !inString;
                }

                result.Append(c);
            }

            // If still inside string → close it
            if (inString)
            {
                result.Append('"');
            }

            return result.ToString();
        }

        public static string EscapeUnbalancedQuotes(string input)
        {
            var result = new System.Text.StringBuilder();
            bool insideString = false;
            int i = 0;

            while (i < input.Length)
            {
                char c = input[i];

                // Handle escape sequences inside a string — skip them as-is
                if (insideString && c == '\\' && i + 1 < input.Length)
                {
                    result.Append(c);
                    result.Append(input[i + 1]);
                    i += 2;
                    continue;
                }

                if (c == '"')
                {
                    if (!insideString)
                    {
                        // Opening quote — enter string mode
                        insideString = true;
                        result.Append(c);
                    }
                    else
                    {
                        // Could be closing quote — peek ahead to decide
                        // A real closing quote is followed by: } ] , \n whitespace
                        int next = i + 1;
                        while (next < input.Length && input[next] == ' ') next++;

                        bool isClosing = next >= input.Length
                            || input[next] == '}'
                            || input[next] == ']'
                            || input[next] == ','
                            || input[next] == '\n'
                            || input[next] == '\r'
                            || input[next] == ':';  // e.g. "key": ...

                        if (isClosing)
                        {
                            insideString = false;
                            result.Append(c);
                        }
                        else
                        {
                            // Unbalanced quote inside a string value — escape it
                            result.Append("\\\"");
                        }
                    }
                }
                else
                {
                    result.Append(c);
                }

                i++;
            }

            // If still inside a string at end of input — close it
            if (insideString)
                result.Append('"');

            return result.ToString();
        }
    }
}
