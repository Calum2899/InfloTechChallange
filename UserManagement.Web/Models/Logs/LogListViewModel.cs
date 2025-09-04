using System;
using UserManagement.Models;

namespace UserManagement.Web.Models.Logs;

public class LogListViewModel
{
    public string SearchTerm { get; set; } = "";
    public string SearchColumn { get; set; } = "Action";
    public string SortBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages { get; set; }
    public bool HasNext => Page < TotalPages;
    public bool HasPrev => Page > 1;
    public List<LogListItemViewModel> Items { get; set; } = new();
}

public class LogListItemViewModel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User user { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string? Action { get; set; }
    public string? Description { get; set; }
    public long ModifiedBy { get; set; }
    public User UserThatModified { get; set; } = null!;
}
