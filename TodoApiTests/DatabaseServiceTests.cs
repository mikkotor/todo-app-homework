using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApiTests;

public class DatabaseServiceTests : IDisposable
{
    private readonly Mock<ILogger<LiteDbService>> _mockLogger = new();
    private readonly LiteDbService? _db;
    private readonly List<TodoList> _todoList = new();

    /// <summary>
    /// Somewhat ugly test written early on to test-drive LiteDB
    /// </summary>
    public DatabaseServiceTests()
    {
        // Delete old db files before test run
        var dir = new DirectoryInfo(".");
        var dbFiles = dir.EnumerateFiles().Where(file => file.Name.Contains(".db"));
        foreach (var dbFile in dbFiles)
        {
            dbFile.Delete();
        }

        _mockLogger = new Mock<ILogger<LiteDbService>>();
        _db = new LiteDbService(_mockLogger.Object);

        // Put some test data in
        _todoList.Add(new TodoList
        {
            Id = 0,
            Name = "New list",
            Todos = new List<Todo>
            {
                new Todo { Description = "Todo 1", IsDone = true },
                new Todo { Description = "Todo 2", IsDone = false }
            }
        });
    }

    public void Dispose()
    {
        _db!.Dispose();
    }

    [Fact]
    public void LiteDbTests()
    {
        // Insert new list, make sure id is correct
        _todoList[0].Id = _db!.InsertTodoList(_todoList[0]);
        Assert.Equal(1, _todoList[0].Id);

        // Get lists, make sure previous insert did it's job
        var getTodoListResult = _db.GetTodoLists().ToArray();
        Assert.Equivalent(_todoList, getTodoListResult);

        // Add another todo list
        _todoList.Add(new TodoList
        {
            Id = 0,
            Name = "New list",
            Todos = new List<Todo>
            {
                new Todo { Description = "Todo 1", IsDone = true },
                new Todo { Description = "Todo 2", IsDone = false }
            }
        });

        // Insert another list, make sure id is correct
        _todoList[1].Id = _db.InsertTodoList(_todoList[1]);
        Assert.Equal(2, _todoList[1].Id);

        // Get lists again, make sure both are in DB
        getTodoListResult = _db.GetTodoLists().ToArray();
        Assert.Equivalent(_todoList, getTodoListResult);

        // Modify one list, update and make sure it succeeds
        _todoList[1].Name = "New name for second list";
        _todoList[1].Todos[1].IsDone = true;
        var updateResult = _db.UpdateTodoList(_todoList[1]);
        Assert.True(updateResult);

        // Get lists yet again, make sure everything still up to date
        getTodoListResult = _db.GetTodoLists().ToArray();
        Assert.Equivalent(_todoList, getTodoListResult);

        // Delete list and ensure it succeeds
        var deleteResult = _db.DeleteTodos(_todoList[1].Id);
        Assert.True(deleteResult);

        // Modify one list, change id to something not present in DB
        _todoList[1].Id = 123;

        // Make sure update fails with wrong id
        updateResult = _db.UpdateTodoList(_todoList[1]);
        Assert.False(updateResult);

        // Make sure delete fails with wrong id
        deleteResult = _db.DeleteTodos(_todoList[1].Id);
        Assert.False(deleteResult);
    }
}
