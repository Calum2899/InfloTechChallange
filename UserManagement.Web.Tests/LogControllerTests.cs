using System;
using System.Collections.Generic;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Logs;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class LogControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var logs = SetupLogs();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<LogListViewModel>()
            .Which.Items.Should().BeEquivalentTo(logs);
    }

    private List<Log> SetupLogs(long userId = 1, string action = "Create", long modifiedBy = 0)
    {
        var logsList = new List<Log>
        {
            new Log
            {
                UserId = userId,
                Action = action,
                Description = $"User action {action} performed.",
                ModifiedBy = modifiedBy,
                Timestamp = DateTime.UtcNow
            }
        };

        _logService.Setup(s => s.GetAll()).Returns(() => logsList);

        return logsList;
    }

    private readonly Mock<ILogService> _logService = new();
    private LogsController CreateController() => new(_logService.Object);
}
