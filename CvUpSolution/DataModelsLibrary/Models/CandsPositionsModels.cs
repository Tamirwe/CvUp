using Database.models;
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
        public DateTime dateCreated { get; set; }
        public string cvKey { get; set; } = "";
        public string tempFilePath { get; set; } = "";
        public string fileExtension { get; set; } = "";
        public string cvTxt { get; set; } = "";
        public string phone { get; set; } = "";
        public string? city { get; set; }
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
        public bool isSameCvEmailSubject { get; set; } = false;
        public int fileTypeKey { get; set; }
        public int? positionTypeId { get; set; }
        public string? body { get; set; }

    }

    [Keyless]
    public class CvsToIndexModel
    {
        //public int companyId { get; set; }
        //public int cvId { get; set; }
        //public string? cvTxt { get; set; } = "";
        //public string? emailSubject { get; set; } = "";
        public int candidateId { get; set; }
        public string? phone { get; set; } = "";
        public string? email { get; set; } = "";
        public string? firstName { get; set; } = "";
        public string? lastName { get; set; } = "";
        public string? reviewText { get; set; } = "";
        public string? cvsTxt { get; set; } = "";

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
        public int? posCvId { get; set; }
        public string? keyId { get; set; } = "";
        public string fileType { get; set; } = "";
        public string? phone { get; set; } = "";
        public string? email { get; set; } = "";
        public string? city { get; set; } = "";
        public string? emailSubject { get; set; } = "";
        public string? review { get; set; } = "";
        public DateTime? reviewDate { get; set; }
        public string? customerReview { get; set; } = "";
        public CandCustomersReviewsModel[]? allCustomersReviews { get; set; }
        public string? firstName { get; set; } = "";
        public string? lastName { get; set; } = "";
        public bool hasDuplicates { get; set; }
        public DateTime cvSent { get; set; }
        public int[]? candPosIds { get; set; } 
        public CandPosStageModel[]? posStages { get; set; }
        public int[]? candFoldersIds { get; set; }
        public bool isSeen { get; set; }
        public int? score { get; set; }

    }

    public class CandReportModel
    {
        public int candidateId { get; set; }
        public string? firstName { get; set; } = "";
        public string? lastName { get; set; } = "";
        public int positionId { get; set; }
        public int? customerId { get; set; } = 0;
        public string? positionName { get; set; }
        public DateTime? stageDate { get; set; }
    }

    public class PositionTypeModel
    {
        public int id { get; set; }
        public string typeName { get; set; } = "";
        public DateTime dateUpdated { get; set; }
    }

    public class PositionTypeCountModel
    {
        public int id { get; set; }
        public string typeName { get; set; } = "";
        public int? todayCount { get; set; }
        public int? yesterdayCount { get; set; }
    }

    public class CandCustomersReviewsModel
    {
        public int candId { get; set; }
        public int posId { get; set; }
        public int custId { get; set; }
        public string posName { get; set; } = "";
        public string custName { get; set; } = "";
        public string? review { get; set; } = "";
        public DateTime? updated { get; set; }
    }

    public class CandDetailsModel
    {
        public int candidateId { get; set; }
        public int companyId { get; set; }
        public string? firstName { get; set; } = "";
        public string? lastName { get; set; } = "";
        public string? email { get; set; } = "";
        public string? phone { get; set; } = "";
    }

    public class CandPosStageTypeModel
    {
        public string stageType { get; set; } = "";
        public int? order { get; set; }
        public string name { get; set; } = "";
        public bool isCustom { get; set; } = false;
        public string? color { get; set; } = "";
        public string? stageEvent { get; set; } = "";
    }

    public class CandPosStageModel
    {
        public int _pid { get; set; } 
        public string? _tp { get; set; } = "";
        public string? _dt { get; set; } = "";
        public string? _ec { get; set; } = "";
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
        public int? positionId { get; set; }
    }

    public class SearchModel
    {
        public string? value { get; set; }
        public string? advanced_value { get; set; }
        public bool? is_exact { get; set; }
    }

    public class searchCandCvModel
    {
        public string value { get; set; } = "";
        public bool exact { get; set; }
        public string advancedValue { get; set; } = "";
        public int folderId { get; set; }
        public int positionId { get; set; }
        public int positionTypeId { get; set; }
    }

    public class AttachePosCandCvModel
    {
        public int companyId { get; set; }
        public int positionId { get; set; }
        public int candidateId { get; set; }
        public int cvId { get; set; }
        public string? keyId { get; set; } = "";
    }

    public class CandPosStatusUpdateCvModel
    {
        public int companyId { get; set; }
        public int positionId { get; set; }
        public int candidateId { get; set; }
        public string stageType { get; set; } = "";
    }

    public class SendEmailModel
    {
        public int companyId { get; set; }
        public int userId { get; set; }
        public int? candidateId { get; set; }
        public int cvId { get; set; }
        public int? positionId { get; set; }
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

    public class EmailTemplateModel
    {
        public int? companyId { get; set; }
        public int id { get; set; }
        public string name { get; set; } = "";
        public string subject { get; set; } = "";
        public string body { get; set; } = "";
        public string? stageToUpdate { get; set; } = "";

    }

    public class PosCvsModel
    {
        public int cvId { get; set; }
        public string keyId { get; set; } = "";
        public bool isSentByEmail { get; set; } = false;
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
        public int company_id { get; set; }
        public int parser_id { get; set; }
        public string delimiter { get; set; } = "";
        public string value_type { get; set; } = "";
        public int order { get; set; }
        public bool must_metch { get; set; }
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
        public string fileName { get; set; } = string.Empty;
    }

}
