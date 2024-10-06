using identity_server.EntityFrameworks.Contexts;
using identity_server.EntityFrameworks.Entities;
using identity_server.Models.Users;
using infrastructures.Services.Base;
using NATS.Client.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace identity_server.Services
{
    public interface IUserService : IBaseService
    {
        Task<UserEntity> Create(RegisterRequest model);
    }

    public class UserService : BaseService, IUserService
    {
        private readonly AppDbContext _appDbContext;
        private readonly NatsConnection _natsConnection;

        public UserService(AppDbContext appDbContext, NatsConnection natsConnection)
        {
            _appDbContext = appDbContext;
            _natsConnection = natsConnection;
        }

        public async Task<UserEntity> Create(RegisterRequest model)
        {
            UserEntity entity = new UserEntity();
            model.MapToEntity(entity);
            await _appDbContext.Set<UserEntity>().AddAsync(entity);
            await _appDbContext.SaveChangesAsync();

            await _natsConnection.PublishAsync("register_user", JsonConvert.SerializeObject(entity));

            return entity;
        }
    }
}
