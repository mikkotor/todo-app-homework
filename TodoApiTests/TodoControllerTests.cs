using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Controllers;
using TodoApi.Filters;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApiTests;

public class TodoControllerTests
{
    private const string TestUserId = "auth0|testuserid";

    [Fact]
    public async Task WhenNoTodoListsInDb_EmptyListReturned()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseServiceAsync>();
        mockDb.Setup(db => db.GetTodoLists(TestUserId)).ReturnsAsync([]);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = await controller.Get();

        Assert.Empty(result.Value!);
        Assert.Single(mockLogger.Invocations);
    }

    [Fact]
    public async Task WhenSomeTodoListsInDb_ControllerReturnsThem()
    {
        var expectedResult = new List<TodoList>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "New list",
                Todos =
                [
                    new Todo { Description = "Todo 1", IsDone = true },
                    new Todo { Description = "Todo 2", IsDone = false },
                ],
            },
        };
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseServiceAsync>();
        mockDb.Setup(db => db.GetTodoLists(TestUserId)).ReturnsAsync(expectedResult);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = await controller.Get();

        Assert.NotEmpty(result.Value!);
        Assert.Single(mockLogger.Invocations);
    }

    [Fact]
    public async Task WhenTryingToAddNewTodoList_WithNonZeroId_ItIsAddedWithZeroAsId()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseServiceAsync>();
        var newTodoListId = Guid.NewGuid();
        mockDb
            .Setup(db => db.InsertTodoList(It.IsAny<TodoList>()))
            .Callback<TodoList>(tl => tl.Id = newTodoListId)
            .ReturnsAsync(newTodoListId);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = await controller.Post(
            new TodoList
            {
                Id = Guid.NewGuid(),
                Name = "New list",
                Todos =
                [
                    new Todo { Description = "Todo 1", IsDone = true },
                    new Todo { Description = "Todo 2", IsDone = false },
                ],
            }
        );

        Assert.Equal(newTodoListId, result.Value);
    }

    [Fact]
    public async Task WhenSuccessfullyUpdatingTodoList_ResultIs200()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseServiceAsync>();
        mockDb.Setup(db => db.UpdateTodoList(It.IsAny<TodoList>())).ReturnsAsync(true);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result =
            await controller.Patch(
                new TodoList
                {
                    Id = Guid.NewGuid(),
                    Name = "New list",
                    Todos =
                    [
                        new Todo { Description = "Todo 1", IsDone = true },
                        new Todo { Description = "Todo 2", IsDone = false },
                    ],
                }
            ) as StatusCodeResult;

        Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
    }

    [Fact]
    public async Task WhenFailingToUpdateTodoList_ResultIs404()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseServiceAsync>();
        mockDb.Setup(db => db.UpdateTodoList(It.IsAny<TodoList>())).ReturnsAsync(false);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result =
            await controller.Patch(
                new TodoList
                {
                    Id = Guid.NewGuid(),
                    Name = "New list",
                    Todos =
                    [
                        new Todo { Description = "Todo 1", IsDone = true },
                        new Todo { Description = "Todo 2", IsDone = false },
                    ],
                }
            ) as StatusCodeResult;

        Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
    }

    [Fact]
    public async Task WhenSuccessfullyDeletingTodoList_ResultIs200()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseServiceAsync>();
        mockDb.Setup(db => db.DeleteTodos(It.IsAny<Guid>())).ReturnsAsync(true);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = await controller.Delete(Guid.NewGuid()) as StatusCodeResult;

        Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
    }

    [Fact]
    public async Task WhenFailingToDeleteTodoList_ResultIs404()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseServiceAsync>();
        mockDb.Setup(db => db.DeleteTodos(It.IsAny<Guid>())).ReturnsAsync(false);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = await controller.Delete(Guid.NewGuid()) as StatusCodeResult;

        Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
    }

    private static TodoController CreateController(
        ILogger<TodoController> logger,
        IDatabaseServiceAsync db
    )
    {
        var controller = new TodoController(logger, db)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Items = { { AuthenticatedAttribute.UserId, TestUserId } },
                },
            },
        };
        return controller;
    }
}
