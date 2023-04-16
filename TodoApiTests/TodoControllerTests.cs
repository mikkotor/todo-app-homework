using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApiTests;

public class TodoControllerTests
{
    [Fact]
    public void WhenNoTodoListsInDb_EmptyListReturned()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.GetTodoLists()).Returns(new List<TodoList>());
        var controller = new TodoController(mockLogger.Object, mockDb.Object);

        var result = controller.Get();

        Assert.Empty(result);
        Assert.Equal(1, mockLogger.Invocations.Count);
    }

    [Fact]
    public void WhenSomeTodoListsInDb_ControllerReturnsThem()
    {
        var expectedResult = new List<TodoList>
        {
            new TodoList
            {
                Id = 1,
                Name = "New list",
                Todos = new List<Todo>
                {
                    new Todo { Description = "Todo 1", IsDone = true },
                    new Todo { Description = "Todo 2", IsDone = false }
                }
            }
        };
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.GetTodoLists()).Returns(expectedResult);
        var controller = new TodoController(mockLogger.Object, mockDb.Object);

        var result = controller.Get();

        Assert.NotEmpty(result);
        Assert.Equal(1, mockLogger.Invocations.Count);
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
        var controller = new TodoController(mockLogger.Object, mockDb.Object);

        var result = controller.Post(new TodoList
        {
            Id = 2345,
            Name = "New list",
            Todos = new List<Todo>
            {
                new Todo { Description = "Todo 1", IsDone = true },
                new Todo { Description = "Todo 2", IsDone = false }
            }
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
        var controller = new TodoController(mockLogger.Object, mockDb.Object);

        var result = controller.Patch(new TodoList
        {
            Id = 2345,
            Name = "New list",
            Todos = new List<Todo>
            {
                new Todo { Description = "Todo 1", IsDone = true },
                new Todo { Description = "Todo 2", IsDone = false }
            }
        });

        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public void WhenFailingToUpdateTodoList_ResultIs404()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.UpdateTodoList(It.IsAny<TodoList>()))
            .Returns(false);
        var controller = new TodoController(mockLogger.Object, mockDb.Object);

        var result = controller.Patch(new TodoList
        {
            Id = 2345,
            Name = "New list",
            Todos = new List<Todo>
            {
                new Todo { Description = "Todo 1", IsDone = true },
                new Todo { Description = "Todo 2", IsDone = false }
            }
        });

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public void WhenSuccessfullyDeletingTodoList_ResultIs200()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.DeleteTodos(It.IsAny<int>()))
            .Returns(true);
        var controller = new TodoController(mockLogger.Object, mockDb.Object);

        var result = controller.Delete(1);

        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public void WhenFailingToDeleteTodoList_ResultIs404()
    {
        var mockLogger = new Mock<ILogger<TodoController>>();
        var mockDb = new Mock<IDatabaseService>();
        mockDb.Setup(db => db.DeleteTodos(It.IsAny<int>()))
            .Returns(false);
        var controller = new TodoController(mockLogger.Object, mockDb.Object);

        var result = controller.Delete(1);

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }
}
