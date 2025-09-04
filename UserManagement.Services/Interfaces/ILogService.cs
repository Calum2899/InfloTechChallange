using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces
{
    public interface ILogService
    {
        /// <summary>
        /// Get all logs
        /// </summary>
        /// <returns></returns>
        IEnumerable<Log> GetAll();
        IEnumerable<Log> GetByUserId(long userId);
        Log GetById(long id);
    }
}
