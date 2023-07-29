using EmailsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Models
{
    public class ImportCvModel
    {
        public int companyId { get; set; }
        public int cvId { get; set; }
        public string cvKey { get; set; } = "";
        public string tempFilePath { get; set; } = "";
        public string fileExtension { get; set; } = "";
        public string cvTxt { get; set; } = "";
        public string phone { get; set; } = "";
        public string emailAddress { get; set; } = "";
        public int candidateId { get; set; } = 0;
        public bool isNewCandidate { get; set; } = false;
        public string candidateName { get; set; } = "";
        public string firstName { get; set; } = "";
        public string lastName { get; set; } = "";
        public string emailId { get; set; } = "";
        public string subject { get; set; } = "";
        public string from { get; set; } = "";
        public string positionRelated { get; set; } = "";
        public int cvAsciiSum { get; set; }
        public bool isDuplicate { get; set; } = false;
        public int duplicateCvId { get; set; }
        public bool isSameCv { get; set; } = false;
    }

    [Keyless]
    public class CvsToIndexModel
    {
        public int companyId { get; set; }
        public int cvId { get; set; }
        public int candidateId { get; set; }
        //public string cvKey { get; set; } = "";
        public string? cvTxt { get; set; } = "";
        public string? phone { get; set; } = "";
        public string? email { get; set; } = "";
        public string? emailSubject { get; set; } = "";
        public string? firstName { get; set; } = "";
        public string? lastName { get; set; } = "";
        public string? reviewText { get; set; } = "";
    }

    public class CvPropsToIndexModel
    {
        public int id { get; set; }
        public int companyId { get; set; }
        public int candidateId { get; set; }
        public string? cvTxt { get; set; } = "";
        public int? cvdbid { get; set; }
    }

    public class CandCvModel
    {
        public int candidateId { get; set; }
        public int cvId { get; set; }
        public string? keyId { get; set; } = "";
        public string? emailSubject { get; set; } = "";
        public DateTime cvSent { get; set; }

    }

    public class CandModel
    {
        public int candidateId { get; set; }
        public int cvId { get; set; }
        public string? keyId { get; set; } = "";
        public string fileType { get; set; } = "";
        public string? phone { get; set; } = "";
        public string? email { get; set; } = "";
        public string? emailSubject { get; set; } = "";
        public string? review { get; set; } = "";
        public string? firstName { get; set; } = "";
        public string? lastName { get; set; } = "";
        public bool hasDuplicates { get; set; }
        public DateTime cvSent { get; set; }
        public int[]? candPosIds { get; set; } 
        public int[]? cvPosIds { get; set; }
        public CandPosStageModel[]? posStages { get; set; }
        public int? stageId { get; set; }
        public DateTime dateAttached { get; set; }
        public List<PosCandCvsModel>? candCvs { get; set; }
        public int folderCandId { get; set; }
    }

    public class CandDetailsModel
    {
        public int candidateId { get; set; }
        public int? companyId { get; set; }
        public string? firstName { get; set; } = "";
        public string? lastName { get; set; } = "";
        public string? email { get; set; } = "";
        public string? phone { get; set; } = "";
    }

    public class companyStagesTypesModel
    {
        public string stageType { get; set; } = "";
        public int? order { get; set; }
        public string name { get; set; } = "";
        public bool isCustom { get; set; } = false;
        public string? color { get; set; } = "";
    }

    public class CandPosStageModel
    {
        public int id { get; set; } 
        public string? t { get; set; } = "";
        public string? d { get; set; } = "";

    }

    public class PosCandCvsModel
    {
        public int cvId { get; set; }
        public string? keyId { get; set; }
        public bool isSentByEmail { get; set; } 
    }

    public class CvReviewModel
    {
        public int candidateId { get; set; }
        public int cvId { get; set; }
        public string reviewHtml { get; set; } = "";
        public string reviewText { get; set; } = "";
    }

    public class CandReviewModel
    {
        public int candidateId { get; set; }
        public string review { get; set; } = "";
    }

    public class searchCandCvModel
    {
        public string keyWords { get; set; } = "";
        public int folderId { get; set; }
        public int positionId { get; set; }
        public bool? isProximitySearch { get; } = false;
    }

    public class AttachePosCandCvModel
    {
        public int companyId { get; set; }
        public int positionId { get; set; }
        public int candidateId { get; set; }
        public int cvId { get; set; }
        public string? keyId { get; set; } = "";
    }

    public class EmailToCandModel
    {
        public int companyId { get; set; }
        public int positionId { get; set; }
        public int candidateId { get; set; }
        public List<EmailAddress> addresses { get; set; }= new List<EmailAddress>();
        public string emailSubject { get; set; } = "";
        public string emailBody { get; set; } = "";
    }

    public class EmailTemplateModel
    {
        public int? companyId { get; set; }
        public int id { get; set; }
        public string name { get; set; } = "";
        public string subject { get; set; } = "";
        public string body { get; set; } = "";
    }

    public class PosCvsModel
    {
        public int cvId { get; set; }
        public string keyId { get; set; } = "";
        public bool isSentByEmail { get; set; } = false;
    }

    public class CandPosModel
    {
        public List<int> candPosIds { get; set; }=new List<int> { };
        public List<int> cvPosIds { get; set; } = new List<int> { };
    }

    public class CvModel
    {
        public int candId { get; set; }
        public int cvId { get; set; }
        public string? reviewHtml { get; set; } = "";
        public string? reviewText { get; set; } = "";
    }

    public class ParserRulesModel
    {
        public int parser_id { get; set; }
        public string delimiter { get; set; } = "";
        public string value_type { get; set; } = "";
        public int order { get; set; }
        public bool must_metch { get; set; }
    }

    public class SendEmailModel
    {
        public int companyId { get; set; }
        public List<EmailAddress>? toAddresses { get; set; }
        public string? subject { get; set; } = "";
        public string? body { get; set; } = "";
        public List<EmailCvAttachmentModel>? attachCvs { get; set; }

        //public int? candId { get; set; }
        //public int? cvId { get; set; }
        //public int? positionId { get; set; } = 0;
        //public string? positionName { get; set; } = string.Empty;
        //public string? customerName { get; set; } = string.Empty;
        //public int? customerId { get; set; } = 0;
        
    }

    public class EmailCvAttachmentModel
    {
        public string cvKey { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
    }

    public class CvFileDetailsModel
    {
        public string cvFilePath { get; set; } = string.Empty;
        public string cvFileType { get; set; } = string.Empty;
    }

}
