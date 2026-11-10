using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ToDoList.Api.Domain;

namespace ToDoList.Api.DTOs
{
    public class PatchTodoDto
    {
        [MinLength(1, ErrorMessage = "Title cannot be empty or whitespace.")]
        [RegularExpression(@"[\s\S]*\S[\s\S]*",
            ErrorMessage = "Title cannot be empty or whitespace.")]
        [StringLength(TodoConstraints.TitleMaxLength, 
            ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }
        private string? _description;
        [StringLength(TodoConstraints.DescriptionMaxLength, 
            ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description 
        { 
            get => _description;
            set
            {
                _description = string.IsNullOrWhiteSpace(value) 
                    ? null
                    : value;
                DescriptionSpecified = true;
            }
        }
        [JsonIgnore]
        public bool DescriptionSpecified { get; private set; }
        public bool? IsCompleted { get; set; }
        private DateTime? _deadline;
        public DateTime? Deadline
        {
            get => _deadline;
            set
            {
                _deadline = value;
                DeadlineSpecified = true;
            }
        }

        [JsonIgnore]
        public bool DeadlineSpecified { get; private set; }

    }
}
