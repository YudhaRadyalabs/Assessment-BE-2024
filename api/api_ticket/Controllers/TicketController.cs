using api_ticket.Models.Tickets;
using api_ticket.Services;
using infrastructures.Models;
using infrastructures.Models.Paginations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api_ticket.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateTicketRequest model)
        {
            try
            {
                var result = await _ticketService.Create(model);

                return new SuccessApiResponse("sukses", result);
            }
            catch (Exception ex)
            {
                return new ErrorApiResponse(ex.InnerException == null ? ex.Message + " : " + JsonConvert.SerializeObject(ex) : ex.InnerException.Message + " : " + JsonConvert.SerializeObject(ex.InnerException));
            }
        }
    }
}
