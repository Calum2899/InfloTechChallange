using System;

namespace UserManagement.Web.Models.Logs;

public class LogListViewModel
{
    public List<LogListItemViewModel> Items { get; set; } = new();
}

public class LogListItemViewModel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Action { get; set; }
    public string? Description { get; set; }
    public long ModifiedBy { get; set; }
}
