using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ToDoList.Api.DTOs
{
    public class PatchTodoDto
    {
        [RegularExpression(@"[\s\S]*\S[\s\S]*",
            ErrorMessage = "Title cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }
        private string? _description;
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
        public DateTime? Deadline { get; set; }

    }
}
