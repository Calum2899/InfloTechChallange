using System.Linq;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Logs;
using UserManagement.Web.Models.Users;

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

    [HttpGet]
    public IActionResult DetailsOfSelectedLog(long id)
    {
        var log = _logService.GetById(id);
        if (log == null) return NotFound();

        var model = new LogListItemViewModel
        {
            Id = log.Id,
            UserId = log.UserId,
            Action = log.Action,
            Description = log.Description,
            Timestamp = log.Timestamp,
            ModifiedBy = log.ModifiedBy
        };

        return View("LogDetails", model);
    }

    private static LogListViewModel CreateModel(IEnumerable<LogListItemViewModel> items) => new LogListViewModel { Items = items.ToList() };

}
