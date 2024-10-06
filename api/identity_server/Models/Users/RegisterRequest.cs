using identity_server.EntityFrameworks.Entities;

namespace identity_server.Models.Users
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }

        public void MapToEntity(UserEntity entity)
        {
            entity.Fullname = Fullname;
            entity.Username = Username;
            entity.Password = Password;
            entity.Email = Email;
            entity.PhoneNo = PhoneNumber;
            entity.IsAdmin = IsAdmin;
        }
    }
}