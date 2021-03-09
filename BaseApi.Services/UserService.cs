using System;
using System.Collections.Generic;
using System.Text;
using BaseApi.Data;
using System.Linq;
namespace BaseApi.Services
{
    public interface IUserService
    {
    }
    public class UserService : IUserService
    {
        private readonly BaseApiDbContext _baseApiDbContext;
        public UserService(BaseApiDbContext baseApiDbContext)
        {
            _baseApiDbContext = baseApiDbContext;

        }
    }
}
