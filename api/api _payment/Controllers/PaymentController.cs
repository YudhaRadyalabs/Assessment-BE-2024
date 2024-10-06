using infrastructures.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NATS.Client.Core;
using Newtonsoft.Json;

namespace api__payment.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly NatsConnection _natsConnection;
        public PaymentController()
        {

        }

        [HttpPost("success")]
        public async Task<ActionResult> CreateSuccess(Guid ticketId)
        {
            try
            {
                await _natsConnection.PublishAsync("confiramtion_payment", JsonConvert.SerializeObject(new { Id = ticketId, IsSuccess = true }));

                return new SuccessApiResponse("sukses");
            }
            catch (Exception ex)
            {
                return new ErrorApiResponse(ex.InnerException == null ? ex.Message + " : " + JsonConvert.SerializeObject(ex) : ex.InnerException.Message + " : " + JsonConvert.SerializeObject(ex.InnerException));
            }
        }

        [HttpPost("fail")]
        public async Task<ActionResult> Createfailed(Guid ticketId)
        {
            try
            {
                await _natsConnection.PublishAsync("confiramtion_payment", JsonConvert.SerializeObject(new { Id = ticketId, IsSuccess = true }));

                return new SuccessApiResponse("sukses");
            }
            catch (Exception ex)
            {
                return new ErrorApiResponse(ex.InnerException == null ? ex.Message + " : " + JsonConvert.SerializeObject(ex) : ex.InnerException.Message + " : " + JsonConvert.SerializeObject(ex.InnerException));
            }
        }
    }
}
