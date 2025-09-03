using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }
    [Fact]
    public void List_WhenServiceReturnsActiveUsers_ModelMustContainActiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List(true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users.Where(u => u.IsActive == true));
    }
    [Fact]
    public void List_WhenServiceReturnsInactiveUsers_ModelMustContainInactiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List(false);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users.Where(u => u.IsActive == false));
    }

    [Fact]
    public void Create_WhenServiceReturnsUsers_ModelMustContainNewUser()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers(); // Sets up a backing list and mocks Create/GetAll

        // Act: Invokes the method under test with the arranged parameters.
        var resultAction = controller.Create(new UserFormViewModel
        {
            Forename = "New",
            Surname = "User",
            DateOfBirth = new DateOnly(1990, 01, 01),
            Email = "test@test.com",
            IsActive = true
        });

        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle(u => u.Forename == "New" && u.Surname == "User");
    }
    [Fact]
    public void Edit_WhenServiceReturnsUsers_ModelMustContainEditedUser()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        var originalUser = users[0];

        var updatedModel = new UserFormViewModel
        {
            Id = originalUser.Id,
            Forename = "Updated",
            Surname = "User",
            DateOfBirth = new DateOnly(1995, 05, 05),
            Email = "updated@test.com",
            IsActive = false
        };

        // Act: Invokes the method under test with the arranged parameters.
        controller.Edit(updatedModel);

        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle(u =>
                u.Id == originalUser.Id &&
                u.Forename == "Updated" &&
                u.Surname == "User" &&
                u.DateOfBirth == new DateOnly(1995, 05, 05) &&
                u.Email == "updated@test.com" &&
                u.IsActive == false
            );
    }
    [Fact]
    public void Delete_WhenServiceCalled_RemovesUserFromList()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers(); // backing list with one user

        var userToDelete = users[0];

        // Setup Delete callback to remove the user from the list
        _userService.Setup(s => s.Delete(It.IsAny<User>()))
                    .Callback<User>(u =>
                    {
                        users.RemoveAll(x => x.Id == u.Id);
                    });

        // Act: Invokes the method under test with the arranged parameters.
        controller.Delete(userToDelete.Id);

        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().NotContain(u => u.Id == userToDelete.Id);
    }

    private List<User> SetupUsers(string forename = "Johnny", string surname = "User",
        DateOnly? dateOfBirth = null, string email = "juser@example.com", bool isActive = true)
    {
        var userList = new List<User>
        {
            new User
            {
                Id = 1,
                Forename = forename,
                Surname = surname,
                DateOfBirth = dateOfBirth ?? new DateOnly(2000, 01, 01),
                Email = email,
                IsActive = isActive
            }
        };

        _userService.Setup(s => s.GetAll()).Returns(() => userList);

        _userService.Setup(s => s.Create(It.IsAny<User>())).Callback<User>(u =>
                    {
                        u.Id = userList.Count > 0 ? userList.Max(x => x.Id) + 1 : 1;
                        userList.Add(u);
                    });


        _userService.Setup(s => s.Update(It.IsAny<User>())).Callback<User>(u =>
                    {
                        var existing = userList.FirstOrDefault(x => x.Id == u.Id);
                        if (existing != null)
                        {
                            existing.Forename = u.Forename;
                            existing.Surname = u.Surname;
                            existing.DateOfBirth = u.DateOfBirth;
                            existing.Email = u.Email;
                            existing.IsActive = u.IsActive;
                        }
                    });
        _userService.Setup(s => s.GetById(It.IsAny<int>())).Returns<int>(id => userList.FirstOrDefault(u => u.Id == id));
        _userService.Setup(s => s.Delete(It.IsAny<User>())).Callback<User>(u =>
        {
            userList.RemoveAll(x => x.Id == u.Id);
        });

        return userList;
    }

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}
