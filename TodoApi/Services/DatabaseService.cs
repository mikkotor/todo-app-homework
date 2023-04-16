using LiteDB;
using TodoApi.Models;

namespace TodoApi.Services;

public interface IDatabaseService
{
    /// <summary>
    /// Get all todo lists from the database
    /// </summary>
    /// <returns>Todo lists stored in DB</returns>
    IEnumerable<TodoList> GetTodoLists();

    /// <summary>
    /// Insert a brand new todo list to database. Make sure id is 0
    /// </summary>
    /// <param name="todos">The new todo list</param>
    /// <returns>New id of the list that was saved</returns>
    int InsertTodoList(TodoList todos);

    /// <summary>
    /// Update todo list
    /// </summary>
    /// <param name="todos">The todo list to update</param>
    /// <returns>True if successful, false if todo list couldn't be found</returns>
    bool UpdateTodoList(TodoList todos);

    /// <summary>
    /// Delete todo list
    /// </summary>
    /// <param name="todoId">The todo list to delete</param>
    /// <returns>True if list deleted, false if not</returns>
    bool DeleteTodos(int todoId);
}

public class LiteDbService : IDatabaseService, IDisposable
{
    private readonly ILogger<LiteDbService> _logger;
    private readonly LiteDatabase _db;
    private readonly ILiteCollection<TodoList> _collection;

    public LiteDbService(ILogger<LiteDbService> logger)
    {
        _logger = logger;
        _db = new LiteDatabase(@"TodoData.db");
        _collection = _db.GetCollection<TodoList>("todos");
        _collection.EnsureIndex(x => x.Name);
    }

    public IEnumerable<TodoList> GetTodoLists()
    {
        try
        {
            var result = _collection.Query();
            return result != null ? result.ToEnumerable() : Array.Empty<TodoList>();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to get any todo lists from database");
            throw;
        }
    }

    public int InsertTodoList(TodoList todos)
    {
        try
        {
            var result = _collection.Insert(todos);
            return (int)result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert a new todo list to database");
            throw;
        }
    }

    public bool UpdateTodoList(TodoList todos)
    {
        try
        {
            var result = _collection.Update(todos);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to update todo list with id {todos.Id}");
            throw;
        }
    }

    public bool DeleteTodos(int todoId)
    {
        try
        {
            return _collection.Delete((BsonValue)todoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete todo list with id {todoId}");
            throw;
        }
    }

    public void Dispose() => _db.Dispose();
}
