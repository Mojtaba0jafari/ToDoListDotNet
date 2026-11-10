using System.ComponentModel.DataAnnotations;
using ToDoList.Api.Domain;

namespace ToDoList.Api.DTOs
{
    public class CreateTodoDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(TodoConstraints.TitleMaxLength,
            ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;
        [StringLength(TodoConstraints.DescriptionMaxLength, 
            ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description {  get; set; }
        public DateTime? Deadline { get; set; }
    }
}
