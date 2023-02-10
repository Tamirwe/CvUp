﻿using System;
using System.Collections.Generic;
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
        public string emailId { get; set; } = "";
        public string subject { get; set; } = "";
        public string from { get; set; } = "";
        public string positionRelated { get; set; } = "";
        public int cvAsciiSum { get; set; }
        public bool isDuplicate { get; set; } = false;
        public int duplicateCvId { get; set; }
        public bool isSameCv { get; set; } = false;
    }

    public class CvPropsToIndexModel
    {
        public int companyId { get; set; }
        public int cvId { get; set; }
        public string cvKey { get; set; } = "";
        public string? cvTxt { get; set; } = "";
        public string? phone { get; set; } = "";
        public string? email { get; set; } = "";
        public string? emailSubject { get; set; } = "";
        public string? candName { get; set; } = "";
        public string? reviewText { get; set; } = "";
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
        public string? candidateName { get; set; } = "";
        public bool hasDuplicates { get; set; }
        public DateTime cvSent { get; set; }
        public int[]? candPosIds { get; set; } 
        public int[]? cvPosIds { get; set; }
        public int? stageId { get; set; }
        public DateTime dateAttached { get; set; }
        public List<PosCandCvsModel>? candCvs { get; set; }

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

    public class AttachePosCandCvModel
    {
        public int companyId { get; set; }
        public int positionId { get; set; }
        public int candidateId { get; set; }
        public int cvId { get; set; }
        public string? keyId { get; set; } = "";
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

}