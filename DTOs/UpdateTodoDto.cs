using System.ComponentModel.DataAnnotations;

namespace ToDoList.Api.DTOs
{
    public class UpdateTodoDto
    {
        [Required(ErrorMessage ="Title is required.")]
        [StringLength(100, ErrorMessage ="Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;
        public string? Description {  get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
