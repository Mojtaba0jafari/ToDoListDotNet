using System.ComponentModel.DataAnnotations;

namespace ToDoList.Api.DTOs
{
    public class CreateTodoDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exced 100 characters.")]
        public string Title { get; set; } = string.Empty;
        public string? Description {  get; set; }
        public DateTime? Deadline { get; set; }
    }
}
