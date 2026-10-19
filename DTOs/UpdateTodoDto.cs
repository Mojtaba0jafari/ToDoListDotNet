namespace ToDoList.Api.DTOs
{
    public class UpdateTodoDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description {  get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
