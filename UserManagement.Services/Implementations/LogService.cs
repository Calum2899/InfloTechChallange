using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;


namespace UserManagement.Services.Domain.Implementations
{
    public class LogService:ILogService
    {
        private readonly IDataContext _dataAccess;
        public LogService(IDataContext dataAccess) => _dataAccess = dataAccess;

        public IEnumerable<Log> GetAll() => _dataAccess.GetAll<Log>();


        public IEnumerable<Log> GetByUserId(long userId)
        {
            return _dataAccess.GetAll<Log>()
                 .Include(l => l.User)
                 .Include(l => l.UserThatModified);
        }

        public Log GetById(long id)
        {
            return _dataAccess.GetAll<Log>()
                .Include(l => l.User)
                .Include(l => l.UserThatModified)
                .First(l => l.Id == id);
        }
    }
}
