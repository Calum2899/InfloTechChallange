using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers
{
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

        private static UserListViewModel CreateModel(IEnumerable<UserListItemViewModel> items) => new UserListViewModel { Items = items.ToList() };

        [HttpGet]
        public IActionResult Create()
        {
            var model = new UserFormViewModel
            {
                Forename = string.Empty,
                Surname = string.Empty,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now), 
                Email = string.Empty,
                IsActive = true
            };

            return View("CreateUserForm",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateUserForm", model);
            }

            var user = new User
            {
                Forename = model.Forename,
                Surname = model.Surname,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                IsActive = model.IsActive
            };

            _userService.Create(user);

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public IActionResult Edit(long id)
        {
            var user = _userService.GetById((int)id);
            if (user == null) return NotFound();

            var model = new UserFormViewModel
            {
                Id = user.Id,
                Forename = user.Forename,
                Surname = user.Surname,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                IsActive = user.IsActive
            };

            return View("EditUserForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserFormViewModel model)
        {
            if (!ModelState.IsValid) return View("EditUserForm", model);

            var user = new User
            {
                Id = model.Id,
                Forename = model.Forename,
                Surname = model.Surname,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                IsActive = model.IsActive
            };

            _userService.Update(user);

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public IActionResult View(long id)
        {
            var user = _userService.GetById((int)id);
            if (user == null) return NotFound();

            var model = new UserFormViewModel
            {
                Id = user.Id,
                Forename = user.Forename,
                Surname = user.Surname,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                IsActive = user.IsActive
            };

            return View("ViewUserForm", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(long id)
        {
            var user = _userService.GetById((int)id);
            if (user == null) return NotFound();

            _userService.Delete(user);

            return RedirectToAction(nameof(List));
        }
    }
}
