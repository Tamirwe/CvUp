using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAiLibrary
{
    public enum Seniority { Junior, Mid, Senior, Lead, Unknown }

    internal static class ParseAiResult
    {
        private static string limitLen(string? original, int maxLength)
        {
            if (original != null)
            {
                return original.Substring(0, Math.Min(original.Length, maxLength));
            }
            return "";
        }

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

            try
            {

                var obj = JObject.Parse(json);

                return new AnalyzedCvModel
                {
                    Name = limitLen(obj.Value<string>("name"), 101),
                    Email = limitLen(obj.Value<string>("email"), 150),
                    Phone = limitLen(obj.Value<string>("phone"), 20),
                    Location = limitLen(obj.Value<string>("location"), 50),
                    CurrentTitle = limitLen(obj.Value<string>("current_title"), 50),
                    YearsExperience = obj.Value<int>("years_experience"),
                    Skills = obj["skills"]?.ToObject<List<string>>() ?? [],
                    Languages = obj["languages"]?.ToObject<List<string>>() ?? [],
                    Seniority = obj.Value<string>("seniority") ?? "Unknown",
                    Summary = limitLen(obj.Value<string>("summary"), 1000),

                };


                //var raw = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json)
                //          ?? throw new Exception("Empty JSON");

                //return new CvAnalysisResult
                //{
                //    FirstName = JsonHelpers.GetString(raw,"first_name"),
                //    LastName = JsonHelpers.GetString(raw,"last_name"),
                //    FullName = JsonHelpers.GetString(raw,"full_name"),
                //    Email = JsonHelpers.GetString(raw,"email"),
                //    Phone = JsonHelpers.GetString(raw,"phone"),
                //    Location = JsonHelpers.GetString(raw,"location"),
                //    CurrentTitle = JsonHelpers.GetString(raw,"current_title"),
                //    YearsExperience = JsonHelpers.GetInt(raw,"years_experience"),
                //    Skills = JsonHelpers.GetStringList(raw,"skills"),
                //    Languages = JsonHelpers.GetStringList(raw,"languages"),
                //    Seniority = Enum.TryParse<Seniority>(JsonHelpers.GetString(raw,"seniority"), true, out var s)
                //                          ? s : Seniority.Unknown,
                //    Summary = JsonHelpers.GetString(raw,"summary"),

                //};
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [warn] JSON parse failed: {ex.Message}");
                Console.WriteLine($"  [raw]  {json[..Math.Min(200, json.Length)]}");
                throw ex;

            }
        }
    }
}
