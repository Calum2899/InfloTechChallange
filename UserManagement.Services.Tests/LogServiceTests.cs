using System;
using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests;

public class LogServiceTests
{
    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var logs = SetupLogs();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(logs);
    }

    private IQueryable<Log> SetupLogs(long userId = 1, string action = "Create", long modifiedBy = 0)
    {
        var logs = new[]
        {
            new Log
            {
                UserId = userId,
                Action = action,
                Description = $"User action {action} performed.",
                ModifiedBy = modifiedBy,
                Timestamp = DateTime.UtcNow
            }
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<Log>())
            .Returns(logs);

        return logs;
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private LogService CreateService() => new(_dataContext.Object);
}
