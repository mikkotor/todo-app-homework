using LiteDB;
using TodoApi.Models;

namespace TodoApi.Services;

public sealed class LiteDbService : IDatabaseService, IDisposable
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
            return result != null ? result.ToEnumerable() : [];
        }
        catch (Exception ex)
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
            _logger.LogError(ex, "Failed to update todo list with id {Id}", todos.Id);
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
            _logger.LogError(ex, "Failed to delete todo list with id {Id}", todoId);
            throw;
        }
    }

    public void Dispose() => _db.Dispose();
}
