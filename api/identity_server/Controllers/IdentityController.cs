using identity_server.Models.Auth;
using identity_server.Models.Users;
using identity_server.Services;
using infrastructures.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace identity_server.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public IdentityController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IUserService userService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet("authorize")]
        public IActionResult Authorize()
        {
            var authorizeUrl = $"{_configuration["IssuerUri"]}/connect/authorize";
            return Redirect(authorizeUrl);
        }

        [AllowAnonymous]
        [HttpPost("login_password")]
        public async Task<IActionResult> LoginPassword(LoginRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var requestToken = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["IssuerUri"]}/connect/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("client_id", "assessment_client"),
                new KeyValuePair<string, string>("client_secret", "511536EF-F270-4058-80CA-1C89C192F69A"),
                new KeyValuePair<string, string>("grant_type", "password"),
                //new KeyValuePair<string, string>("scope", "profile"),
                new KeyValuePair<string, string>("username", request.Usermane),
                new KeyValuePair<string, string>("password", request.Password)
            })
            };

            var response = await client.SendAsync(requestToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            else
            {
                return new BadRequestResult();
            }
        }

        [AllowAnonymous]
        [HttpGet("userinfo")]
        public async Task<IActionResult> UserInfo([FromQuery] string token)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["IssuerUri"]}/connect/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequest model)
        {
            try
            {
                var result = await _userService.Create(model);

                return new SuccessApiResponse("sukses", result);
            }
            catch (Exception ex)
            {
                return new ErrorApiResponse(ex.InnerException == null ? ex.Message + " : " + JsonConvert.SerializeObject(ex) : ex.InnerException.Message + " : " + JsonConvert.SerializeObject(ex.InnerException));
            }
        }

        [HttpGet("test")]
        public ActionResult TestAuth()
        {
            try
            {
                return new SuccessApiResponse("sukses");
            }
            catch (Exception ex)
            {
                return new ErrorApiResponse(ex.InnerException == null ? ex.Message + " : " + JsonConvert.SerializeObject(ex) : ex.InnerException.Message + " : " + JsonConvert.SerializeObject(ex.InnerException));
            }
        }

    }
}
