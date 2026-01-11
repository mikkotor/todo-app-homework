using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
public class TodoController(ILogger<TodoController> logger, IDatabaseService db) : ControllerBase
{
    /// <summary>
    /// Get all todo lists from database
    /// </summary>
    /// <returns>All todo lists in database or empty enumerable if none are found</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoList>), StatusCodes.Status200OK)]
    public IEnumerable<TodoList> Get()
    {
        var todos = db.GetTodoLists();
        logger.LogInformation("Found {Count} todo lists", todos.Count());
        return todos;
    }

    /// <summary>
    /// Creates a new todo list. Id of incoming list is always set to 0
    /// </summary>
    /// <param name="todos">Todo list to add</param>
    /// <returns>Returns the id of the new todo list</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public ObjectResult Post(TodoList todos)
    {
        if (todos.Id != 0) todos.Id = 0;
        var newTodoListId = db.InsertTodoList(todos);
        logger.LogInformation("New todo list ADDED with id {Id}", newTodoListId);
        return new OkObjectResult(newTodoListId);
    }

    /// <summary>
    /// Update the todo list
    /// </summary>
    /// <param name="todos">Todo list to update</param>
    /// <returns>200 OK if update successful, 404 NOT FOUND otherwise</returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public StatusCodeResult Patch(TodoList todos)
    {
        var result = db.UpdateTodoList(todos);
        if (result)
        {
            logger.LogInformation("Todo list UPDATED with id {Id}", todos.Id);
            return Ok();
        }
        logger.LogInformation("Todo list with id {Id} NOT FOUND", todos.Id);
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
    public StatusCodeResult Delete(int id)
    {
        var result = db.DeleteTodos(id);
        if (result)
        {
            logger.LogInformation("Todo list DELETED with id {Id}", id);
            return Ok();
        }
        logger.LogInformation("Todo list with id {Id} NOT FOUND", id);
        return new StatusCodeResult(StatusCodes.Status404NotFound);
    }
}
