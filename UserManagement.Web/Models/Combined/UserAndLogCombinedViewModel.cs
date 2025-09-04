using UserManagement.Web.Models.Users;
using UserManagement.Web.Models.Logs;

namespace UserManagement.Web.Models.Combined;

public class UserAndLogCombinedViewModel
{
    public UserFormViewModel User { get; set; } = new UserFormViewModel();
    public LogListViewModel Logs { get; set; } = new LogListViewModel();
}
