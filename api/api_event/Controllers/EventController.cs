using api_event.Models.Events;
using api_event.Service;
using infrastructures.Models;
using infrastructures.Models.Paginations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api_event.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "admin")]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost("datatable")]
        public async Task<ActionResult<PaginationResponse<DatatableEventResponse>>> Getpagination(PaginationRequest request)
        {
            try
            {
                var pagedList = _eventService.Datatable(request);

                return new SuccessApiResponse("sukses", pagedList.Result);
            }
            catch (Exception ex)
            {
                return new ErrorApiResponse(ex.InnerException == null ? ex.Message + " : " + JsonConvert.SerializeObject(ex) : ex.InnerException.Message + " : " + JsonConvert.SerializeObject(ex.InnerException));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Create(Guid id)
        {
            try
            {
                var result = await _eventService.GetById(id);

                return new SuccessApiResponse("sukses", result);
            }
            catch (Exception ex)
            {
                return new ErrorApiResponse(ex.InnerException == null ? ex.Message + " : " + JsonConvert.SerializeObject(ex) : ex.InnerException.Message + " : " + JsonConvert.SerializeObject(ex.InnerException));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateEventRequest model)
        {
            try
            {
                var result = await _eventService.Create(model);

                return new SuccessApiResponse("sukses", result);
            }
            catch (Exception ex)
            {
                return new ErrorApiResponse(ex.InnerException == null ? ex.Message + " : " + JsonConvert.SerializeObject(ex) : ex.InnerException.Message + " : " + JsonConvert.SerializeObject(ex.InnerException));
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(UpdateEventRequest model)
        {
            try
            {
                var result = await _eventService.Update(model);

                return new SuccessApiResponse("sukses", result);
            }
            catch (Exception ex)
            {
                return new ErrorApiResponse(ex.InnerException == null ? ex.Message + " : " + JsonConvert.SerializeObject(ex) : ex.InnerException.Message + " : " + JsonConvert.SerializeObject(ex.InnerException));
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] DeleteEventRequest model)
        {
            try
            {
                await _eventService.Delete(model);

                return new SuccessApiResponse("sukses");
            }
            catch (Exception ex)
            {
                return new ErrorApiResponse(ex.InnerException == null ? ex.Message + " : " + JsonConvert.SerializeObject(ex) : ex.InnerException.Message + " : " + JsonConvert.SerializeObject(ex.InnerException));
            }
        }
    }
}
