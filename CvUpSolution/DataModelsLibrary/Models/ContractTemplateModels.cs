namespace DataModelsLibrary.Models
{
    public class ContractTemplateModel
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public string? descr { get; set; }
        public DateTime dateCreated { get; set; }
    }
}
