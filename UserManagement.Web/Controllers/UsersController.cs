using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users/[action]")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet ]
    public ViewResult List() => View(CreateModel(GetUsers()));
    [HttpGet]
    public ViewResult ListActive() => View("List", CreateModel(GetUsers().Where(u => u.IsActive == true)));
    [HttpGet]
    public ViewResult ListInactive() => View("List", CreateModel(GetUsers().Where(u => u.IsActive == false)));
    private IEnumerable<UserListItemViewModel> GetUsers() => _userService.GetAll().Select(p => new UserListItemViewModel
    {
        Id = p.Id,
        Forename = p.Forename,
        Surname = p.Surname,
        Email = p.Email,
        IsActive = p.IsActive
    });
    private static UserListViewModel CreateModel(IEnumerable<UserListItemViewModel> items) => new UserListViewModel
    {
        Items = items.ToList()
    };

}
