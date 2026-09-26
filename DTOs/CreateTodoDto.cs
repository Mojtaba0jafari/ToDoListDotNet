namespace ToDoList.Api.DTOs
{
    public class CreateTodoDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description {  get; set; }
        public DateTime? Deadline { get; set; }
    }
}
