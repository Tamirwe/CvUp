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
    }
}
