using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Logs;

namespace UserManagement.WebMS.Controllers;

[Route("logs/[action]")]
public class LogsController: Controller
{
    private readonly ILogService _logService;

    public LogsController(ILogService logService) => _logService = logService;

    [HttpGet]
    public ViewResult List() => View(CreateModel(GetLogs()));

    private IEnumerable<LogListItemViewModel> GetLogs() => _logService.GetAll().Select(p => new LogListItemViewModel
    {
        Id = p.Id,
        UserId = p.UserId,
        Action = p.Action,
        Description = p.Description,
        Timestamp = p.Timestamp,
        ModifiedBy = p.ModifiedBy
    });
    private static LogListViewModel CreateModel(IEnumerable<LogListItemViewModel> items) => new LogListViewModel { Items = items.ToList() };

}
