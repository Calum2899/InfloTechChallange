using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations
{
    public class UserService : IUserService
    {
        private readonly IDataContext _dataAccess;
        public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

        public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();

        /// <summary>
        /// Return users by active state
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public IEnumerable<User> FilterByActive(bool isActive) => _dataAccess.GetAll<User>().Where(user => user.IsActive == isActive);

        public User? GetById(int id) => _dataAccess.GetAll<User>().FirstOrDefault(u => u.Id == id);

        public void Create(User user, long modifiedBy) => _dataAccess.Create(user, modifiedBy);

        public void Update(User user, long modifiedBy) => _dataAccess.Update(user, modifiedBy);

        public void Delete(User user, long modifiedBy) => _dataAccess.Delete(user, modifiedBy);
    }
}
