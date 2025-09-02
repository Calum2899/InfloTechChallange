using System;
using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users/[action]")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public ViewResult List(bool? isActive = null)
    {
        var users = GetUsers();

        if (isActive.HasValue)
            users = users.Where(u => u.IsActive == isActive.Value);

        return View(CreateModel(users));
    }
    private IEnumerable<UserListItemViewModel> GetUsers() => _userService.GetAll().Select(p => new UserListItemViewModel
    {
        Id = p.Id,
        Forename = p.Forename,
        Surname = p.Surname,
        Email = p.Email,
        DateOfBirth = p.DateOfBirth,
        IsActive = p.IsActive
    });
    private static UserListViewModel CreateModel(IEnumerable<UserListItemViewModel> items) => new UserListViewModel
    {
        Items = items.ToList()
    };

    void AddUser(string forename, string surname, DateOnly dateOfBirth, string email, bool isActive = true)
    {
        _userService.Create(new Models.User
        {
            Forename = forename,
            Surname = surname,
            DateOfBirth = dateOfBirth,
            Email = email,
            IsActive = isActive
        });
    }
}
