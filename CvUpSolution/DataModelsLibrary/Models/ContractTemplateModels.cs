namespace DataModelsLibrary.Models
{
    public class ContractTemplateModel
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public string? descr { get; set; }
        public DateTime dateCreated { get; set; }
    }

    public class SendContractModel
    {
        public int contractTemplateId { get; set; }
        public string companyName { get; set; } = "";
        public string? companyAddress { get; set; }
        // The customer's business registration number, not the CvUp company
        public string? companyIdNumber { get; set; }
        public string? contactName { get; set; }
        public string emailAddress { get; set; } = "";
        public string? emailSubject { get; set; }
        public string? emailBody { get; set; }
    }

    public class ContractSendModel
    {
        public int id { get; set; }
        public string contractTemplateName { get; set; } = "";
        public string? contractTemplateDescr { get; set; }
        public string companyName { get; set; } = "";
        public string? companyAddress { get; set; }
        public string? companyIdNumber { get; set; }
        public string? contactName { get; set; }
        public string emailAddress { get; set; } = "";
        public string? emailSubject { get; set; }
        public string? emailBody { get; set; }
        public DateTime dateSent { get; set; }
    }

    public class ContractTemplateFileModel
    {
        public Stream content { get; set; } = Stream.Null;
        public string contentType { get; set; } = "";
    }
}
