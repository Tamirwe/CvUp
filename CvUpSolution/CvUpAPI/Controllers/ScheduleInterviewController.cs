using AuthLibrary;
using CandsPositionsLibrary;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ScheduleInterviewController : ControllerBase
    {
        private IScheduleInterviewService _scheduleInterviewService;
        private IAuthServise _authServise;

        public ScheduleInterviewController(IScheduleInterviewService scheduleInterviewService, IAuthServise authServise)
        {
            _scheduleInterviewService = scheduleInterviewService;
            _authServise = authServise;
        }

        [HttpGet]
        [Route("GetScheduleInterview")]
        public async Task<IActionResult> GetScheduleInterview(int? positionId, int? candidateId)
        {
            ScheduleInterviewModel? scheduleInterview = await _scheduleInterviewService.GetScheduleInterview(Globals.CompanyId, positionId, candidateId);
            return Ok(scheduleInterview);
        }

        [HttpPost]
        [Route("AddScheduleInterview")]
        public async Task<IActionResult> AddScheduleInterview(ScheduleInterviewModel scheduleInterview)
        {
            scheduleInterview.companyId = Globals.CompanyId;
            scheduleInterview.userId = Globals.UserId;

            UserModel? user = await _authServise.GetUser(Globals.CompanyId, Globals.UserId);
            var scheduleInterviewId = await _scheduleInterviewService.AddScheduleInterview(scheduleInterview, user);

            return Ok(scheduleInterviewId);
        }

        [HttpPut]
        [Route("UpdateScheduleInterview")]
        public async Task<IActionResult> UpdateScheduleInterview(ScheduleInterviewModel scheduleInterview)
        {
            scheduleInterview.companyId = Globals.CompanyId;
            scheduleInterview.userId = Globals.UserId;

            UserModel? user = await _authServise.GetUser(Globals.CompanyId, Globals.UserId);
            await _scheduleInterviewService.UpdateScheduleInterview(scheduleInterview, user);

            return Ok();
        }

        [HttpPost]
        [Route("CancelScheduleInterview")]
        public async Task<IActionResult> CancelScheduleInterview(ScheduleInterviewModel scheduleInterview)
        {
            scheduleInterview.companyId = Globals.CompanyId;
            scheduleInterview.userId = Globals.UserId;

            UserModel? user = await _authServise.GetUser(Globals.CompanyId, Globals.UserId);
            await _scheduleInterviewService.CancelScheduleInterview(scheduleInterview, user);

            return Ok();
        }
    }
}
