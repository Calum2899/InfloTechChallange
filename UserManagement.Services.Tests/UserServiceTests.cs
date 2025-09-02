using System;
using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }
    [Fact]
    public void GetActive_WhenParametersAreTrue_MustReturnOnlyActiveEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.FilterByActive(true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEquivalentTo(users.Where(u => u.IsActive == true));
    }
    [Fact]
    public void GetActive_WhenParametersAreFalse_MustReturnOnlyInactiveEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.FilterByActive(false);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEquivalentTo(users.Where(u => u.IsActive == false));
    }

    private IQueryable<User> SetupUsers(string forename = "Johnny", string surname = "User", DateOnly? dateOfBirth = null, string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                DateOfBirth = dateOfBirth ?? new DateOnly(2000, 01, 01),
                Email = email,
                IsActive = isActive
            }
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(users);

        return users;
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);
}
