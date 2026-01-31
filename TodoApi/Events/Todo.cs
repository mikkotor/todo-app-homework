using TodoApi.Models;

namespace TodoApi.Events;

public record TodoListCreated
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public List<Todo>? Todos { get; init; } = [];
}

public record TodoListUpdated
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public List<Todo>? Todos { get; init; } = [];
}

public record TodoListDeleted
{
    public int Id { get; init; }
}
