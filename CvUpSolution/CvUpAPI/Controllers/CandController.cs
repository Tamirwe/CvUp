using AuthLibrary;
using CandsPositionsLibrary;
using CvFilesLibrary;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiLibrary;
using AiLibrary.SearchCvs;
using System.Buffers;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CandController : ControllerBase
    {
        private ICandsServise _candsService;
        private ICandsListsServise _candsListsService;
        private IPositionsServise _positionsService;
        private IAuthServise _authServise;
        private ICvsFilesService _cvsFilesService;
        private ISearchCvsService _aiSearchCvsService;

        public CandController(ICandsServise candsService, ICandsListsServise candsListsService, IPositionsServise positionsService,
            IAuthServise authServise, ICvsFilesService cvsFilesService, ISearchCvsService aiSearchCvsService)
        {
            _candsService = candsService;
            _candsListsService = candsListsService;
            _positionsService = positionsService;
            _authServise = authServise;
            _cvsFilesService = cvsFilesService;
            _aiSearchCvsService = aiSearchCvsService;
        }

        [HttpGet]
        [Route("GetIsAuthorized")]
        public IActionResult GetIsAuthorized()
        {
            return Ok();
        }

        [HttpGet]
        [Route("GetCandsList")]
        public async Task<List<CandModel?>> GetCandsList(int page = 1, int take = 200)
        {
            return await _candsListsService.GetCandsList(Globals.CompanyId, null);
        }

        [HttpGet]
        [Route("GetFolderCandsList")]
        public async Task<List<CandModel?>> GetFolderCandsList(int folderId)
        {
            return await _candsListsService.GetFolderCandsList(Globals.CompanyId, folderId);
        }

        [HttpGet]
        [Route("GetPosTypeCandsList")]
        public async Task<List<CandModel?>> GetPosTypeCandsList(int positionTypeId)
        {
            return await _candsListsService.GetPosTypeCandsList(Globals.CompanyId, positionTypeId);
        }

        [HttpPost]
        [Route("SearchCands")]
        public async Task<IEnumerable<CandModel?>> SearchCands(searchCandCvModel searchVals)
        {
            List<CandModel?> candsList = new List<CandModel?>();
            var results = await _candsService.SearchCands(Globals.CompanyId, searchVals);

            var candsIds = results.Select(e => e.Id).ToList();

            if (searchVals.folderId > 0)
            {
                candsList = await _candsListsService.GetFolderCandsList(Globals.CompanyId, searchVals.folderId);
                candsList = candsList.Where(x => x != null && candsIds.Any(y => y == x.candidateId)).ToList();
            }
            else if (searchVals.positionId > 0)
            {
                candsList = await _candsListsService.GetPosCandsList(Globals.CompanyId, searchVals.positionId);
                candsList = candsList.Where(x => x != null && candsIds.Any(y => y == x.candidateId)).ToList();
            }
            else if (searchVals.positionTypeId > 0)
            {
                candsList = await _candsListsService.GetPosTypeCandsList(Globals.CompanyId, searchVals.positionTypeId);
                candsList = candsList.Where(x => x != null && candsIds.Any(y => y == x.candidateId)).ToList();
            }
            else
            {
                var firstRows = candsIds.GetRange(0, candsIds.Count > 300 ? 300: candsIds.Count);
                candsList = await _candsListsService.GetCandsList(Globals.CompanyId, firstRows);
            }

            foreach (var res in results)
            {
                var itemToChange = candsList.FirstOrDefault(x => x != null && x.candidateId == res.Id);

                if (itemToChange != null)
                {
                    itemToChange.score = res.Score;
                }
            }

            var sortedCands = candsList.OrderByDescending(x => x?.score).ToList();
            return sortedCands;
        }

        [HttpPost]
        [Route("ComplexSearchCands")]
        public async Task<IEnumerable<CandModel?>> ComplexSearchCands([FromBody] ComplexSearchRequest request)
        {
            var results = await _candsListsService.ComplexSearchCands(Globals.CompanyId, request.FirstSearch, request.SearchWithin);

            if (results.Count == 0)
                return [];

            var candsIds = results.Select(e => e.Id).ToList();
            var firstRows = candsIds.GetRange(0, candsIds.Count > 300 ? 300 : candsIds.Count);
            var candsList = await _candsListsService.GetCandsList(Globals.CompanyId, firstRows);

            foreach (var res in results)
            {
                var itemToChange = candsList.FirstOrDefault(x => x != null && x.candidateId == res.Id);
                if (itemToChange != null)
                    itemToChange.score = res.Score;
            }

            return candsList.OrderByDescending(x => x?.score).ToList();
        }

        [HttpPost]
        [Route("AiSearchCands")]
        public async Task<List<CandModel>> AiSearchCands(searchCandCvModel searchVals)
        {
            var luceneResults = await _candsService.SearchForAiFilter(searchVals);
            var candidateIds = luceneResults.Select(e => e.Id).ToList();
            var aiResults = await _aiSearchCvsService.SearchCvs(searchVals, candidateIds, limit: 10);
            var candsIds = aiResults.Select(e => e.candidateId).ToList();
            var candsList = await _candsListsService.GetCandsList(Globals.CompanyId, candsIds);
            List<CandModel> results = _candsService.MergeAiResultsWithCandsList(candsList, aiResults);

            return results;
        }

        [HttpGet]
        [Route("GetCandCvsList")]
        public async Task<List<CandCvModel>> GetCandCvsList(int candidateId)
        {
            return await _candsService.GetCandCvsList(Globals.CompanyId, candidateId);
        }

        [HttpGet]
        [Route("GetPositionSearchKeywords")]
        public async Task<AnalyzedPositionModel?> GetPositionSearchKeywords(int positionId)
        {
            return await _candsListsService.GetAnalyzedPosition(positionId);
        }

        [HttpGet]
        [Route("GetPositionSearchTerms")]
        public async Task<SearchTermsModel?> GetPositionSearchTerms(int positionId, bool isReAnalyze = false)
        {
            return await _candsListsService.GetPositionSearchTerms(positionId, isReAnalyze);
        }

        [HttpPost]
        [Route("FindMatchCvsByTerms")]
        public async Task<List<CandModel?>> FindMatchCvsByTerms([FromBody] List<string> terms)
        {
            var luceneResults = await _candsListsService.LuceneFindMatchCvsByTerms(terms);
            var candsIds = luceneResults.Select(e => e.Id).ToList();
            var candsList = await _candsListsService.GetCandsList(Globals.CompanyId, candsIds);
            return candsList;
        }

        [HttpGet]
        [Route("FindPositionMatchCvs")]
        public async Task<List<CandModel>> FindPositionMatchCvs(int positionId)
        {
            var aiResults = await _candsListsService.FindPositionMatchCvs(positionId);
            var candsIds = aiResults.Select(e => e.candidateId).ToList();
            var candsList = await _candsListsService.GetCandsList(Globals.CompanyId, candsIds);
            return _candsService.MergeAiResultsWithCandsList(candsList, aiResults);
        }

        [HttpGet]
        [Route("GetPosCandsList")]
        public async Task<List<CandModel?>> GetPosCandsList(int positionId)
        {
            return await _candsListsService.GetPosCandsList(Globals.CompanyId, positionId);
        }

        [HttpGet]
        [Route("getCv")]
        public async Task<CvModel?> getCv(int cvId)
        {
            return await _candsService.GetCv(cvId, Globals.CompanyId);
        }

        [HttpPut]
        [Route("SaveCandReview")]
        public async Task<CandModel?> SaveCandReview(CandReviewModel candReview)
        {
            await _candsService.SaveCandReview(Globals.CompanyId, candReview);
            return await _candsService.GetCandidate(Globals.CompanyId, candReview.candidateId);
        }

        [HttpPost]
        [Route("AttachPosCandCv")]
        public async Task<CandModel?> AttachPosCandCv(AttachePosCandCvModel posCv)
        {
            posCv.companyId = Globals.CompanyId;
            await _candsService.AttachPosCandCv(posCv);
            await _positionsService.UpdatePositionDate(posCv.companyId, posCv.positionId, true);
            return await _candsService.GetCandidate(posCv.companyId, posCv.candidateId);
        }

        [HttpPost]
        [Route("DetachPosCand")]
        public async Task<CandModel?> DetachPosCand(AttachePosCandCvModel posCv)
        {
            posCv.companyId = Globals.CompanyId;
            await _candsService.DetachPosCand(posCv);
            await _positionsService.UpdatePositionDate(posCv.companyId, posCv.positionId, true);
            return await _candsService.GetCandidate(posCv.companyId, posCv.candidateId);
        }

        [HttpGet]
        [Route("GetCandPosStagesTypes")]
        public async Task<List<CandPosStageTypeModel>> GetCandPosStagesTypes()
        {
            return await _candsService.GetCandPosStagesTypes(Globals.CompanyId);
        }

        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail(SendEmailModel emailData)
        {
            emailData.companyId = Globals.CompanyId;
            UserModel? user = await _authServise.GetUser(Globals.CompanyId, Globals.UserId);
            await _candsService.SendEmail(emailData, user);
            await _candsService.AddSendEmail(emailData, Globals.UserId);
            return Ok();
        }

        [HttpPost]
        [Route("UpdateCandPositionStatus")]
        public async Task<CandModel?> UpdateCandPositionStatus(CandPosStageTypeUpdateModel posStatus)
        {
            posStatus.companyId = Globals.CompanyId;
            await _candsService.UpdateCandPositionStatus(posStatus);
            await _positionsService.UpdatePositionDate(posStatus.companyId, posStatus.positionId, false);
            return await _candsListsService.GetPositionCandidate(posStatus.companyId, posStatus.candidateId, posStatus.positionId);
        }

        [HttpPost]
        [Route("UpdatePosStageDate")]
        public async Task<CandModel?> UpdatePosStageDate(CandPosStageTypeUpdateModel posStatus)
        {
            posStatus.companyId = Globals.CompanyId;
            await _candsService.UpdatePosStageDate(posStatus);
            await _positionsService.UpdatePositionDate(posStatus.companyId, posStatus.positionId, false);
            return await _candsListsService.GetPositionCandidate(posStatus.companyId, posStatus.candidateId, posStatus.positionId);
        }

        [HttpPost]
        [Route("RemovePosStage")]
        public async Task<CandModel?> RemovePosStage(CandPosStageTypeUpdateModel posStatus)
        {
            posStatus.companyId = Globals.CompanyId;
            await _candsService.RemovePosStage(posStatus);
            await _positionsService.UpdatePositionDate(posStatus.companyId, posStatus.positionId, false);
            return await _candsListsService.GetPositionCandidate(posStatus.companyId, posStatus.candidateId, posStatus.positionId);
        }

        [HttpGet]
        [Route("GetEmailTemplates")]
        public async Task<List<EmailTemplateModel>> GetEmailTemplates()
        {
            return await _candsService.GetEmailTemplates(Globals.CompanyId);
        }

        [HttpPost]
        [Route("AddUpdateEmailTemplate")]
        public async Task<IActionResult> AddUpdateEmailTemplate(EmailTemplateModel emailTemplate)
        {
            emailTemplate.companyId = Globals.CompanyId;
            await _candsService.AddUpdateEmailTemplate(emailTemplate);
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteEmailTemplate")]
        public async Task<IActionResult> DeleteEmailTemplate(int id)
        {
            await _candsService.DeleteEmailTemplate(Globals.CompanyId, id);
            return Ok();
        }

        [HttpPut]
        [Route("UpdateCandDetails")]
        public async Task<CandModel?> UpdateCandDetails(CandDetailsModel candDetails)
        {
            candDetails.companyId = Globals.CompanyId;
            await _candsService.UpdateCandDetails(candDetails);
            return await _candsService.GetCandidate(candDetails.companyId, candDetails.candidateId);
        }

        //[HttpGet]
        //[Route("UpdateIsSeen")]
        //public async Task<IActionResult> UpdateIsSeen(int cvId)
        //{
        //    await _candsService.UpdateIsSeen(Globals.CompanyId, cvId);
        //    return Ok();
        //}

        [HttpGet]
        [Route("UpdateIsSeen")]
        public void UpdateIsSeen(int cvId)
        {
            Task backgroundTask = Task.Run(() => _candsService.UpdateIsSeen(Globals.CompanyId, cvId));
        }

        [HttpGet]
        [Route("CandsReport")]
        public async Task<List<CandReportModel?>> CandsReport(string stageType)
        {
            return await _candsService.CandsReport(Globals.CompanyId, stageType);
        }

        [HttpDelete]
        [Route("DeleteCv")]
        public async Task<CandModel?> DeleteCv(int cnid, int cvId)
        {
            await _candsService.DeleteCv(Globals.CompanyId, cnid, cvId);
            return await _candsService.GetCandidate(Globals.CompanyId, cnid);
        }

        [HttpDelete]
        [Route("DeleteCandidate")]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            await _candsService.DeleteCandidate(Globals.CompanyId, id);
            return Ok();
        }

        [HttpPut]
        [Route("SaveCustomerCandReview")]
        public async Task<CandModel?> SaveCustomerCandReview(CandReviewModel customerCandReview)
        {
            await _candsService.SaveCustomerCandReview(Globals.CompanyId, customerCandReview);
            return await _candsService.GetCandidate(Globals.CompanyId, customerCandReview.candidateId);
        }

        [HttpGet]
        [Route("GetSearches")]
        public async Task<List<SearchModel>> GetSearches()
        {
            return await _candsService.GetSearches(Globals.CompanyId);
        }

        [HttpPost]
        [Route("SaveSearch")]
        public async Task<IActionResult> SaveSearch(SearchModel searchVals)
        {
            await _candsService.SaveSearch(Globals.CompanyId, searchVals);
            return Ok();
        }

        [HttpPut]
        [Route("DeleteSearch")]
        public async Task<IActionResult> DeleteSearch(SearchModel searchVals)
        {
            await _candsService.DeleteSearch(Globals.CompanyId, searchVals);
            return Ok();
        }

        [HttpPut]
        [Route("StarSearch")]
        public async Task<IActionResult> StarSearch(SearchModel searchVals)
        {
            await _candsService.StarSearch(Globals.CompanyId, searchVals);
            return Ok();
        }

        [HttpPut]
        [Route("DeleteAllNotStarSearches")]
        public async Task<List<SearchModel>> DeleteAllNotStarSearches()
        {
            await _candsService.DeleteAllNotStarSearches(Globals.CompanyId);
            return await _candsService.GetSearches(Globals.CompanyId);
        }

        [HttpGet]
        [Route("GetKeywordsGroups")]
        public async Task<List<keywordsGroupModel>> GetKeywordsGroups()
        {
            return await _candsService.GetKeywordsGroups(Globals.CompanyId);
        }

        [HttpPost]
        [Route("SaveKeywordsGroup")]
        public async Task<IActionResult> SaveKeywordsGroup(keywordsGroupModel keywordsGroup)
        {
            await _candsService.SaveKeywordsGroup(Globals.CompanyId, keywordsGroup);
            return Ok();
        }

        [HttpPut]
        [Route("DeleteKeywordsGroup")]
        public async Task<IActionResult> DeleteKeywordsGroup(int id)
        {
            await _candsService.DeleteKeywordsGroup(Globals.CompanyId, id);
            return Ok();
        }

        [HttpGet]
        [Route("GetKeywords")]
        public async Task<List<keywordModel>> GetKeywords()
        {
            return await _candsService.GetKeywords(Globals.CompanyId);
        }

        [HttpPost]
        [Route("SaveKeyword")]
        public async Task<IActionResult> SaveKeyword(keywordModel keyword)
        {
            await _candsService.SaveKeyword(Globals.CompanyId, keyword);
            return Ok();
        }

        [HttpPut]
        [Route("DeleteKeyword")]
        public async Task<IActionResult> DeleteKeyword(int id)
        {
            await _candsService.DeleteKeyword(Globals.CompanyId, id);
            return Ok();
        }
    }
}
