using Amazon;
using Amazon.OpenSearchService;
using Amazon.OpenSearchService.Model;
using Newtonsoft.Json;
using System.Net.Mime;
using ThirdParty.Json.LitJson;

using System.Text;
using System.Threading.Tasks;

namespace AwsS3Library
{
   
    public class OpenSearchExample
    {
        private const string DomainName = "your-opensearch-domain-name";
        private const string Region = "your-aws-region";

        public void UploadDocument(string documentId, string documentJson)
        {
            var config = new AmazonOpenSearchServiceConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(Region)
            };

            using (var client = new AmazonOpenSearchServiceClient(config))
            {
                var request = new UploadDocumentsRequest
                {
                    DomainName = DomainName,
                    Documents = new List<string>
                {
                    // Each document must be in the JSON format and include the document ID
                    // The document ID must be unique within the domain
                    // Here, we assume that documentBody is already a valid JSON string
                    $"{{\"id\": \"{documentId}\", {documentBody}}}"
                }
                };

                var response = client.CreatePackageAsync(request);

                // Process the upload response
                // You can access information like response.Status and response.FailedDocuments
            }
        }

        private System.IO.MemoryStream GenerateDocumentStream(string documentId, string documentJson)
        {
            var document = new OpenSearchDocument
            {
                Id = documentId,
                Source = documentJson
            };

            var documents = new OpenSearchDocument[] { document };
            var documentsJson = JsonConvert.SerializeObject(documents);

            var documentBytes = System.Text.Encoding.UTF8.GetBytes(documentsJson);
            var documentStream = new System.IO.MemoryStream(documentBytes);

            return documentStream;
        }
    }

    public class OpenSearchDocument
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string Id { get; set; }

        [Newtonsoft.Json.JsonProperty("source")]
        public string Source { get; set; }
    }

}
