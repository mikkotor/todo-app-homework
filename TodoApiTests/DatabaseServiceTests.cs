using Marten;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApiTests;

public sealed class DatabaseServiceTests
{
    [Fact]
    public async Task GivenNewList_WhenInsertingList_TheIdIsUpdated()
    {
        var todoList = new TodoList
        {
            Id = Guid.NewGuid(),
            Name = "New list",
            Todos =
            [
                new Todo { Description = "Todo 1", IsDone = true },
                new Todo { Description = "Todo 2", IsDone = false },
            ],
        };

        var store = new Mock<IDocumentStore>();
        var session = new Mock<IDocumentSession>();
        session
            .Setup(s => s.Store(It.IsAny<TodoList[]>()))
            .Callback<TodoList[]>(t =>
            {
                t[0].Id = Guid.NewGuid(); // Simulate database assigning an ID
            });
        store.Setup(s => s.LightweightSession()).Returns(() => session.Object);

        var _db = new MartenDbService(new Mock<ILogger<MartenDbService>>().Object, store.Object);

        var newListId = await _db!.InsertTodoList(todoList);
        Assert.Equal(todoList.Id, newListId);
        session.Verify(s => s.Store(It.IsAny<TodoList[]>()), Times.Once);
        session.Verify(s => s.SaveChangesAsync(), Times.Once);

        // // Get lists, make sure previous insert did it's job
        // var getTodoListResult = _db.GetTodoLists().ToArray();
        // Assert.Equivalent(_todoList, getTodoListResult);

        // // Add another todo list
        // _todoList.Add(
        //     new TodoList
        //     {
        //         Id = 0,
        //         Name = "New list",
        //         Todos =
        //         [
        //             new Todo { Description = "Todo 1", IsDone = true },
        //             new Todo { Description = "Todo 2", IsDone = false },
        //         ],
        //     }
        // );

        // // Insert another list, make sure id is correct
        // _todoList[1].Id = _db.InsertTodoList(_todoList[1]);
        // Assert.Equal(2, _todoList[1].Id);

        // // Get lists again, make sure both are in DB
        // getTodoListResult = _db.GetTodoLists().ToArray();
        // Assert.Equivalent(_todoList, getTodoListResult);

        // // Modify one list, update and make sure it succeeds
        // _todoList[1].Name = "New name for second list";
        // _todoList[1].Todos![1].IsDone = true;
        // var updateResult = _db.UpdateTodoList(_todoList[1]);
        // Assert.True(updateResult);

        // // Get lists yet again, make sure everything still up to date
        // getTodoListResult = _db.GetTodoLists().ToArray();
        // Assert.Equivalent(_todoList, getTodoListResult);

        // // Delete list and ensure it succeeds
        // var deleteResult = _db.DeleteTodos(_todoList[1].Id);
        // Assert.True(deleteResult);

        // // Modify one list, change id to something not present in DB
        // _todoList[1].Id = 123;

        // // Make sure update fails with wrong id
        // updateResult = _db.UpdateTodoList(_todoList[1]);
        // Assert.False(updateResult);

        // // Make sure delete fails with wrong id
        // deleteResult = _db.DeleteTodos(_todoList[1].Id);
        // Assert.False(deleteResult);
    }
}
