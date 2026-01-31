using TodoApi.Models;

namespace TodoApi.Services;

public interface IDatabaseServiceAsync
{
    /// <summary>
    /// Get all todo lists from the database where user has access.
    /// </summary>
    /// <param name="userId">The user id to filter todo lists</param>
    /// <returns>Todo lists stored in DB</returns>
    Task<IEnumerable<TodoList>> GetTodoLists(string userId);

    /// <summary>
    /// Insert a brand new todo list to database.
    /// </summary>
    /// <param name="todos">The new todo list</param>
    /// <returns>New id of the list that was saved</returns>
    Task<Guid> InsertTodoList(TodoList todos);

    /// <summary>
    /// Update todo list
    /// </summary>
    /// <param name="todos">The todo list to update</param>
    /// <returns>True if successful, false if todo list couldn't be found</returns>
    Task<bool> UpdateTodoList(TodoList todos);

    /// <summary>
    /// Delete todo list
    /// </summary>
    /// <param name="todoId">The todo list to delete</param>
    /// <returns>True if list deleted, false if not</returns>
    Task<bool> DeleteTodos(Guid todoId);
}
