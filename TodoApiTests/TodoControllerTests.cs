using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;
using TodoApi.Filters;

namespace TodoApiTests;

public class TodoControllerTests
{
    [Fact]
    public void WhenNoTodoListsInDb_EmptyListReturned()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.GetTodoLists()).Returns([]);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = controller.Get();

        Assert.Empty(result.Value!);
        Assert.Single(mockLogger.Invocations);
    }

    [Fact]
    public void WhenSomeTodoListsInDb_ControllerReturnsThem()
    {
        var expectedResult = new List<TodoList>
        {
            new() {
                Id = 1,
                Name = "New list",
                Todos =
                [
                    new Todo { Description = "Todo 1", IsDone = true },
                    new Todo { Description = "Todo 2", IsDone = false }
                ]
            }
        };
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.GetTodoLists()).Returns(expectedResult);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = controller.Get();

        Assert.NotEmpty(result.Value!);
        Assert.Single(mockLogger.Invocations);
    }

    [Fact]
    public void WhenTryingToAddNewTodoList_WithNonZeroId_ItIsAddedWithZeroAsId()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        var erasedId = -1;
        mockDb.Setup(db => db.InsertTodoList(It.IsAny<TodoList>()))
            .Callback<TodoList>(tl => erasedId = tl.Id)
            .Returns(1);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = controller.Post(new TodoList
        {
            Id = 2345,
            Name = "New list",
            Todos =
            [
                new Todo { Description = "Todo 1", IsDone = true },
                new Todo { Description = "Todo 2", IsDone = false }
            ]
        });

        Assert.Equal(1, result.Value);
        Assert.Equal(0, erasedId);
    }

    [Fact]
    public void WhenSuccessfullyUpdatingTodoList_ResultIs200()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.UpdateTodoList(It.IsAny<TodoList>()))
            .Returns(true);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = controller.Patch(new TodoList
        {
            Id = 2345,
            Name = "New list",
            Todos =
            [
                new Todo { Description = "Todo 1", IsDone = true },
                new Todo { Description = "Todo 2", IsDone = false }
            ]
        }) as StatusCodeResult;

        Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
    }

    [Fact]
    public void WhenFailingToUpdateTodoList_ResultIs404()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.UpdateTodoList(It.IsAny<TodoList>()))
            .Returns(false);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = controller.Patch(new TodoList
        {
            Id = 2345,
            Name = "New list",
            Todos =
            [
                new Todo { Description = "Todo 1", IsDone = true },
                new Todo { Description = "Todo 2", IsDone = false }
            ]
        }) as StatusCodeResult;

        Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
    }

    [Fact]
    public void WhenSuccessfullyDeletingTodoList_ResultIs200()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.DeleteTodos(It.IsAny<int>()))
            .Returns(true);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = controller.Delete(1) as StatusCodeResult;

        Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
    }

    [Fact]
    public void WhenFailingToDeleteTodoList_ResultIs404()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.DeleteTodos(It.IsAny<int>()))
            .Returns(false);
        var controller = CreateController(mockLogger.Object, mockDb.Object);

        var result = controller.Delete(1) as StatusCodeResult;

        Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
    }

    private static TodoController CreateController(ILogger<TodoController> logger, IDatabaseService db)
    {
        var controller = new TodoController(logger, db)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Items = { { AuthenticatedAttribute.UserId, "auth0|testuserid" } },
                }
            }
        };
        return controller;
    }
}
