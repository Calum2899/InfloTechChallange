using System;
using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Logs;

namespace UserManagement.WebMS.Controllers;

[Route("logs/[action]")]
public class LogsController : Controller
{
    private readonly ILogService _logService;

    public LogsController(ILogService logService) => _logService = logService;

    [HttpGet]
    public IActionResult List(string searchTerm = "", string searchColumn = "Action", string sortBy = "Id", string sortDirection = "asc", int page = 1, int pageSize = 20)
    {
        var logs = _logService.GetAll();

        // Filter (search)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            switch (searchColumn.ToLower())
            {
                case "userid":
                    if (long.TryParse(searchTerm, out var userId))
                        logs = logs.Where(l => l.UserId == userId);
                    break;
                case "description":
                    logs = logs.Where(l => l.Description != null && l.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                    break;
                default:
                    logs = logs.Where(l => l.Action != null && l.Action.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                    break;
            }
        }

        // Sorting
        logs = sortBy.ToLower() switch
        {
            "userid" => sortDirection == "desc" ? logs.OrderByDescending(l => l.UserId) : logs.OrderBy(l => l.UserId),
            "timestamp" => sortDirection == "desc" ? logs.OrderByDescending(l => l.Timestamp) : logs.OrderBy(l => l.Timestamp),
            "action" => sortDirection == "desc" ? logs.OrderByDescending(l => l.Action) : logs.OrderBy(l => l.Action),
            _ => sortDirection == "desc" ? logs.OrderByDescending(l => l.Id) : logs.OrderBy(l => l.Id)
        };

        // Pagination
        var totalCount = logs.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var pagedLogs = logs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var model = new LogListViewModel
        {
            SearchTerm = searchTerm,
            SearchColumn = searchColumn,
            SortBy = sortBy,
            SortDirection = sortDirection,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            Items = pagedLogs.Select(p => new LogListItemViewModel
            {
                Id = p.Id,
                UserId = p.UserId,
                user = p.User,
                Timestamp = p.Timestamp,
                Action = p.Action,
                Description = p.Description,
                ModifiedBy = p.ModifiedBy,
                UserThatModified = p.UserThatModified
            }).ToList()
        };

        return View(model);
    }



    [HttpGet]
    public IActionResult DetailsOfSelectedLog(long id)
    {
        try
        {
            var log = _logService.GetById(id);

            var model = new LogListItemViewModel
            {
                Id = log.Id,
                UserId = log.UserId,
                user = log.User,
                Action = log.Action,
                Description = log.Description,
                Timestamp = log.Timestamp,
                ModifiedBy = log.ModifiedBy,
                UserThatModified = log.UserThatModified
            };

            return View("LogDetails", model);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
