namespace TodoApi.Models;

public class Todo
{
    public Todo()
    {
        this.Description = "";
    }

    public string Description { get; set; }
    public bool IsDone { get; set; }
}

public class TodoList
{
    public TodoList()
    {
        this.Name = "";
        this.Todos = new List<Todo>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public List<Todo>? Todos { get; set; }
}
