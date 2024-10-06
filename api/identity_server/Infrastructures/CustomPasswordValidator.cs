using identity_server.EntityFrameworks.Contexts;
using identity_server.EntityFrameworks.Entities;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace identity_server.Infrastructures
{
    public class CustomPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly AppDbContext _dbContext;
        
        public CustomPasswordValidator(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _dbContext.Set<UserEntity>().FirstOrDefaultAsync(x => x.Username == context.UserName && x.Password == context.Password);
            if (user != null)
            {
                var claims = new List<Claim>() {
                        new Claim("id", user.Id.ToString()),
                        new Claim("fullname", user.Fullname.ToString()),
                        new Claim("username", user.Username.ToString()),
                        new Claim("email", user.Email.ToString()),
                        new Claim("phone_no", user.PhoneNo.ToString()),
                        new Claim("policy_user", user.IsAdmin ? "admin" : "enduser")
                };
                context.Result = new GrantValidationResult(context.UserName, "password", claims: claims);
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.UnauthorizedClient);
            }

            return;
        }
    }
}
