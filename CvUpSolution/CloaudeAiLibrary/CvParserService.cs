using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using CloaudeAiLibrary.Models;
using DataModelsLibrary.Queries;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace CloaudeAiLibrary
{
    public class CvParserService: ICvParserService
    {
        private readonly HttpClient _http;


        private readonly AnthropicClient _claude;

        public CvParserService(AnthropicClient claude)
        {
            _claude = claude;
            //_candsCvsQueries = candsCvsQueries;
        }

        public async Task<ParsedCvModel> ParseAsync(string rawCvText)
        {
            string content = $$"""
                        Extract the following fields from this CV text.
                        The CV may be in Hebrew, English, or both.

                        Return ONLY a valid JSON object with these exact fields, 
                        no explanation, no markdown:
                        {
                            "full_name": "",
                            "email": "",
                            "phone": "",
                            "city": "",
                            "profession": "",
                            "summary": "",
                            "job_titles": [],
                            "skills": [],
                            "experience_years": null,
                            "education": "",
                            "military_service": "",
                            "languages": "",
                            "enriched_text": ""
                        }

                        For enriched_text: write a rich paragraph combining all possible
                        ways to describe this candidate — include Hebrew and English
                        synonyms for their profession, skills, and titles.
                        This will be used for semantic search.

                        CV Text:
                        {{rawCvText}}
                        """;

            var response = await _claude.Messages.GetClaudeMessageAsync(new MessageParameters
            {
                Model = AnthropicModels.Claude45Haiku,
                MaxTokens = 1000,
                Messages = new List<Message>
                {
                    new Message(RoleType.User, content)
                }
            });

            var json = response.Content.OfType<TextContent>()
                .First().Text
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            json = EscapeUnbalancedQuotes(json);
            json = FixUnbalancedCurlyBraces(json);





            try
            {

                //    var obj = JObject.Parse(json);

                //    return new ParsedCvModel
                //    {
                //        FullName = obj.Value<string>("full_name") ?? "",
                //        Email = obj.Value<string>("email") ?? "",
                //        Phone = obj.Value<string>("phone") ?? "",
                //        City = obj.Value<string>("city") ?? "",
                //        Profession = obj.Value<string>("profession") ?? "",
                //        Summary = obj.Value<string>("summary") ?? "",
                //        JobTitles = obj["job_titles"].ToObject<List<string>>() ?? [],
                //        Skills = obj["skills"].ToObject<List<string>>() ?? [],
                //        ExperienceYears = obj.Value<int>("experience_years") ,
                //        Education = obj.Value<string>("education") ?? "",
                //        MilitaryService = obj.Value<string>("military_service") ?? "",
                //        Languages = obj.Value<string>("languages") ?? "",
                //        EnrichedText = obj.Value<string>("enriched_text") ?? ""

                //        //Name = obj.Value<string>("name"),
                //        //Email = obj.Value<string>("email"),
                //        //Phone = obj.Value<string>("phone"),
                //        //Location = obj.Value<string>("city_he"),
                //        //CurrentTitleEn = obj.Value<string>("current_title_en"),
                //        //CurrentTitleHe = obj.Value<string>("current_title_he"),
                //        //Companies = obj["companies"]?.ToObject<List<string>>() ?? [],
                //        //Skills = obj["skills"]?.ToObject<List<string>>() ?? [],
                //        //SummaryEn = obj.Value<string>("summary_en") ?? "",
                //        //SummaryHe = obj.Value<string>("summary_he") ?? "",
                //        //YearsExperience = myParseInt(obj.Value<string>("years_experience")),
                //        //Languages = obj.Value<string>("languages"),
                //        //Profession = obj.Value<string>("profession_he"),
                //        //Education = obj.Value<string>("education_he"),
                //        //MilitaryService = obj.Value<string>("military_service_he"),
                //        //Seniority = obj.Value<string>("seniority_he"),
                //    };


                var parsed = JsonSerializer.Deserialize<CvJson>(json)!;

                return new ParsedCvModel
                {
                    FullName = parsed.full_name,
                    Email = parsed.email,
                    Phone = parsed.phone,
                    City = parsed.city,
                    Profession = parsed.profession,
                    Summary = parsed.summary,
                    JobTitles = parsed.job_titles ?? new(),
                    Skills = parsed.skills ?? new(),
                    ExperienceYears = parsed.experience_years,
                    Education = parsed.education,
                    MilitaryService = parsed.military_service,
                    Languages = parsed.languages,
                    EnrichedText = parsed.enriched_text
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [warn] JSON parse failed: {ex.Message}");
                Console.WriteLine($"  [raw]  {json[..Math.Min(200, json.Length)]}");
                throw ex;

            }



        
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

        public static  string FixUnbalancedCurlyBraces(string input)
        {
            int open = 0;

            foreach (char c in input)
            {
                if (c == '{') open++;
                else if (c == '}')
                {
                    if (open > 0) open--;
                    // else: extra closing brace — handled below
                }
            }

            // Count extra closing braces (unopened ones)
            int extraClosing = 0;
            int balance = 0;
            foreach (char c in input)
            {
                if (c == '{') balance++;
                else if (c == '}')
                {
                    if (balance > 0) balance--;
                    else extraClosing++;
                }
            }

            var sb = new StringBuilder();

            // Remove extra closing braces
            int skipClosing = extraClosing;
            foreach (char c in input)
            {
                if (c == '}' && skipClosing > 0)
                {
                    skipClosing--;
                    continue;
                }
                sb.Append(c);
            }

            // Append missing closing braces
            for (int i = 0; i < open; i++)
                sb.Append('}');

            return sb.ToString();
        }

        private record CvJson(
            string full_name,
            string email,
            string phone,
            string city,
            string profession,
            string summary,
            List<string> job_titles,
            List<string> skills,
            int? experience_years,
            string education,
            string military_service,
            string languages,
            string enriched_text
        );
    }
}
