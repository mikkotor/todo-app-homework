using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Filters;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

/// <summary>
/// Controller for managing todo lists.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TodoController"/> class.
/// </remarks>
/// <param name="logger">Logger instance.</param>
/// <param name="db">Database service.</param>
[ApiController]
[Route("[controller]")]
[Authorize]
[Authenticated]
public class TodoController(ILogger<TodoController> logger, IDatabaseServiceAsync db)
    : ControllerBase
{
    /// <summary>
    /// Get all todo lists for the current user from database
    /// </summary>
    /// <returns>All todo lists in database or empty enumerable if none are found</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IList<TodoList>), StatusCodes.Status200OK, "application/json")]
    public async Task<ActionResult<IList<TodoList>>> Get()
    {
        var userId = HttpContext.Items[AuthenticatedAttribute.UserId] as string;

        var todos = await db.GetTodoLists(userId!);
        logger.LogInformation("User {UserId} found {Count} todo lists", userId, todos.Count());
        return todos.ToList();
    }

    /// <summary>
    /// Creates a new todo list.
    /// </summary>
    /// <param name="todoList">Todo list to add</param>
    /// <returns>Returns the id of the new todo list</returns>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "text/plain")]
    public async Task<ActionResult<string>> Post(TodoList todoList)
    {
        var userId = HttpContext.Items[AuthenticatedAttribute.UserId] as string;

        todoList.Id = Guid.NewGuid();
        todoList.UserId = userId!;
        var newTodoListId = await db.InsertTodoList(todoList);
        logger.LogInformation(
            "User {UserId} added new todo list with id {Id}",
            userId,
            newTodoListId
        );
        return newTodoListId.ToString();
    }

    /// <summary>
    /// Update the todo list
    /// </summary>
    /// <param name="todos">Todo list to update</param>
    /// <returns>200 OK if update successful, 404 NOT FOUND otherwise</returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Patch(TodoList todos)
    {
        var userId = HttpContext.Items[AuthenticatedAttribute.UserId] as string;

        var result = await db.UpdateTodoList(todos);
        if (result)
        {
            logger.LogInformation("User {UserId} updated todo list with id {Id}", userId, todos.Id);
            return Ok();
        }
        logger.LogInformation(
            "User {UserId} attempted to update non-existent todo list with id {Id}",
            userId,
            todos.Id
        );
        return NotFound();
    }

    /// <summary>
    /// Delete the todo list with the given id
    /// </summary>
    /// <param name="id">Id of the list to delete</param>
    /// <returns>200 OK if update successful, 404 NOT FOUND otherwise</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = HttpContext.Items[AuthenticatedAttribute.UserId] as string;

        var result = await db.DeleteTodos(id);
        if (result)
        {
            logger.LogInformation("User {UserId} deleted todo list with id {Id}", userId, id);
            return Ok();
        }
        logger.LogInformation(
            "User {UserId} attempted to delete non-existent todo list with id {Id}",
            userId,
            id
        );
        return NotFound();
    }
}
