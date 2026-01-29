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
