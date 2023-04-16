using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly IDatabaseService _db;

    public TodoController(ILogger<TodoController> logger, IDatabaseService db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Get all todo lists from database
    /// </summary>
    /// <returns>All todo lists in database or empty enumerable if none are found</returns>
    [HttpGet]
    public IEnumerable<TodoList> Get()
    {
        var todos = _db.GetTodoLists();
        _logger.LogInformation($"Found {todos.Count()} todo lists");
        return todos;
    }

    /// <summary>
    /// Creates a new todo list. Id of incoming list is always set to 0
    /// </summary>
    /// <param name="todos">Todo list to add</param>
    /// <returns>Returns the id of the new todo list</returns>
    [HttpPost]
    public ObjectResult Post(TodoList todos)
    {
        if (todos.Id != 0) todos.Id = 0;
        var newTodoListId = _db.InsertTodoList(todos);
        _logger.LogInformation($"New todo list added with id {newTodoListId}");
        return new OkObjectResult(newTodoListId);
    }

    /// <summary>
    /// Update the todo list
    /// </summary>
    /// <param name="todos">Todo list to update</param>
    /// <returns>200 OK if update successful, 404 NOT FOUND otherwise</returns>
    [HttpPatch]
    public StatusCodeResult Patch(TodoList todos)
    {
        var result = _db.UpdateTodoList(todos);
        if (result) return Ok();
        return new StatusCodeResult(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Delete the todo list with the given id
    /// </summary>
    /// <param name="id">Id of the list to delete</param>
    /// <returns>200 OK if update successful, 404 NOT FOUND otherwise</returns>
    [HttpDelete]
    public StatusCodeResult Delete(int id)
    {
        var result = _db.DeleteTodos(id);
        if (result) return Ok();
        return new StatusCodeResult(StatusCodes.Status404NotFound);
    }
}
