using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TodoApi.Models;
using TodoApi.Services;
using TodoApi.Filters;

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
public class TodoController(ILogger<TodoController> logger, IDatabaseService db) : ControllerBase
{
    /// <summary>
    /// Get all todo lists from database
    /// </summary>
    /// <returns>All todo lists in database or empty enumerable if none are found</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoList>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<TodoList>> Get()
    {
        var userId = HttpContext.Items[AuthenticatedAttribute.UserId] as string;

        var todos = db.GetTodoLists();
        logger.LogInformation("User {UserId} found {Count} todo lists", userId, todos.Count());
        return new ActionResult<IEnumerable<TodoList>>(todos);
    }

    /// <summary>
    /// Creates a new todo list. Id of incoming list is always set to 0
    /// </summary>
    /// <param name="todos">Todo list to add</param>
    /// <returns>Returns the id of the new todo list</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public ActionResult<int> Post(TodoList todos)
    {
        var userId = HttpContext.Items[AuthenticatedAttribute.UserId] as string;

        if (todos.Id != 0) todos.Id = 0;
        var newTodoListId = db.InsertTodoList(todos);
        logger.LogInformation("User {UserId} added new todo list with id {Id}", userId, newTodoListId);
        return new ActionResult<int>(newTodoListId);
    }

    /// <summary>
    /// Update the todo list
    /// </summary>
    /// <param name="todos">Todo list to update</param>
    /// <returns>200 OK if update successful, 404 NOT FOUND otherwise</returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Patch(TodoList todos)
    {
        var userId = HttpContext.Items[AuthenticatedAttribute.UserId] as string;

        var result = db.UpdateTodoList(todos);
        if (result)
        {
            logger.LogInformation("User {UserId} updated todo list with id {Id}", userId, todos.Id);
            return Ok();
        }
        logger.LogInformation("User {UserId} attempted to update non-existent todo list with id {Id}", userId, todos.Id);
        return new StatusCodeResult(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Delete the todo list with the given id
    /// </summary>
    /// <param name="id">Id of the list to delete</param>
    /// <returns>200 OK if update successful, 404 NOT FOUND otherwise</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Delete(int id)
    {
        var userId = HttpContext.Items[AuthenticatedAttribute.UserId] as string;

        var result = db.DeleteTodos(id);
        if (result)
        {
            logger.LogInformation("User {UserId} deleted todo list with id {Id}", userId, id);
            return Ok();
        }
        logger.LogInformation("User {UserId} attempted to delete non-existent todo list with id {Id}", userId, id);
        return new StatusCodeResult(StatusCodes.Status404NotFound);
    }
}
