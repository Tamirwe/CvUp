using System;
using System.Collections.Generic;

namespace DataModelsLibrary.Models
{
    public class ContractTemplateModel
    {
        public int id { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string? descr { get; set; } = string.Empty;
        public DateTime? dateCreated { get; set; }
        public DateTime? dateUpdated { get; set; }

        // False when the row has no .docx behind it in the company folder. The
        // module keeps the row visible but will not let it be sent.
        public bool fileExists { get; set; }
    }

    public class CustomerContractModel
    {
        public int id { get; set; } = 0;
        public int customerId { get; set; }
        public string customerName { get; set; } = string.Empty;
        public int? contactId { get; set; }
        public string contactName { get; set; } = string.Empty;
        public string? contactEmail { get; set; } = string.Empty;
        public int? contractTemplateId { get; set; }
        public string contractTemplateName { get; set; } = string.Empty;
        public string? descr { get; set; } = string.Empty;
        public string? emailSubject { get; set; } = string.Empty;
        public string? emailBody { get; set; } = string.Empty;
        public DateTime? dateSent { get; set; }
        public DateTime? dateCreated { get; set; }
    }

    // What the .docx merge and the email need about one customer + contact.
    public class ContractMergeDataModel
    {
        public string customerName { get; set; } = string.Empty;
        public string? customerAddress { get; set; } = string.Empty;
        public string? customerDescr { get; set; } = string.Empty;
        public string contactName { get; set; } = string.Empty;
        public string contactEmail { get; set; } = string.Empty;
    }

    public class SendContractModel
    {
        public int customerId { get; set; }
        public int contactId { get; set; }
        public int contractTemplateId { get; set; }
        public string? descr { get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
    }
}
