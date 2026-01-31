using Marten;
using TodoApi.Models;

namespace TodoApi.Services;

public class MartenDbService(ILogger<MartenDbService> logger, IDocumentStore store)
    : IDatabaseServiceAsync
{
    public async Task<IEnumerable<TodoList>> GetTodoLists(string userId)
    {
        try
        {
            using var session = store.LightweightSession();
            return
            [
                .. await session.LoadManyAsync<TodoList>(
                    session.Query<TodoList>().Where(t => t.UserId == userId).Select(t => t.Id)
                ),
            ];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving todo lists from database");
            throw;
        }
    }

    public async Task<Guid> InsertTodoList(TodoList todos)
    {
        try
        {
            using var session = store.LightweightSession();
            session.Store(todos);
            await session.SaveChangesAsync();
            return todos.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inserting todo list into database");
            throw;
        }
    }

    public async Task<bool> UpdateTodoList(TodoList todos)
    {
        try
        {
            using var session = store.LightweightSession();
            session.Store(todos);
            await session.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating todo list in database");
            throw;
        }
    }

    public async Task<bool> DeleteTodos(Guid id)
    {
        try
        {
            using var session = store.LightweightSession();
            var todo = await session.LoadAsync<TodoList>(id);
            if (todo == null)
                return false;
            session.Delete<TodoList>(id);
            await session.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting todo list from database");
            throw;
        }
    }
}
