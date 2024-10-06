using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace infrastructures.Services
{
    public interface ICurrentUserService
    {
        public Guid Id { get; }
        public string Fullname { get; }
        public string Username { get; }
        public string Email { get; }
        public string PhoneNo { get; }
    }
    
    internal class CurrentUserService : ICurrentUserService
    {
        public Guid Id { get; }
        public string Fullname { get; }
        public string Username { get; }
        public string Email { get; }
        public string PhoneNo { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            Id = string.IsNullOrEmpty(httpContextAccessor.HttpContext?.User?.FindFirstValue("id")) ? Guid.Empty :
    Guid.Parse(httpContextAccessor.HttpContext?.User?.FindFirstValue("id"));
            Fullname = httpContextAccessor.HttpContext?.User?.FindFirstValue("fullname");
            Username = httpContextAccessor.HttpContext?.User?.FindFirstValue("username");
            Email = httpContextAccessor.HttpContext?.User?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            PhoneNo = httpContextAccessor.HttpContext?.User?.FindFirstValue("phone_no");
        }
    }
}