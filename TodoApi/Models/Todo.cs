namespace TodoApi.Models;

/// <summary>
/// Represents a todo item with a description and completion status.
/// </summary>
public record Todo
{
    /// <summary>
    /// Unique identifier for the todo item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Description of the todo item.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Indicates whether the todo item is completed.
    /// </summary>
    public bool IsDone { get; set; }
}

/// <summary>
/// Represents a list of todo items.
/// </summary>
public record TodoList
{
    /// <summary>
    /// Unique identifier for the todo list.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the todo list.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Collection of todo items in the list.
    /// </summary>
    public List<Todo>? Todos { get; set; } = [];

    /// <summary>
    /// Identifier of the user who owns the todo list.
    /// </summary>
    public string UserId { get; set; } = "";
}
