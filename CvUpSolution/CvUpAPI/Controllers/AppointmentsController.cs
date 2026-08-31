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
    public class AppointmentsController : ControllerBase
    {
        private IAppointmentsService _appointmentsService;
        private IAuthServise _authServise;

        public AppointmentsController(IAppointmentsService appointmentsService, IAuthServise authServise)
        {
            _appointmentsService = appointmentsService;
            _authServise = authServise;
        }

        [HttpGet]
        [Route("GetAppointment")]
        public async Task<IActionResult> GetAppointment(int? positionId, int? candidateId)
        {
            AppointmentModel? appointment = await _appointmentsService.GetAppointment(Globals.CompanyId, positionId, candidateId);
            return Ok(appointment);
        }

        [HttpPost]
        [Route("AddAppointment")]
        public async Task<IActionResult> AddAppointment(AppointmentModel appointment)
        {
            appointment.companyId = Globals.CompanyId;
            appointment.userId = Globals.UserId;

            UserModel? user = await _authServise.GetUser(Globals.CompanyId, Globals.UserId);
            var appointmentId = await _appointmentsService.AddAppointment(appointment, user);

            return Ok(appointmentId);
        }

        [HttpPut]
        [Route("UpdateAppointment")]
        public async Task<IActionResult> UpdateAppointment(AppointmentModel appointment)
        {
            appointment.companyId = Globals.CompanyId;
            appointment.userId = Globals.UserId;

            UserModel? user = await _authServise.GetUser(Globals.CompanyId, Globals.UserId);
            await _appointmentsService.UpdateAppointment(appointment, user);

            return Ok();
        }

        [HttpPost]
        [Route("CancelAppointment")]
        public async Task<IActionResult> CancelAppointment(AppointmentModel appointment)
        {
            appointment.companyId = Globals.CompanyId;
            appointment.userId = Globals.UserId;

            UserModel? user = await _authServise.GetUser(Globals.CompanyId, Globals.UserId);
            await _appointmentsService.CancelAppointment(appointment, user);

            return Ok();
        }
    }
}
