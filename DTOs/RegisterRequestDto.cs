using System.ComponentModel.DataAnnotations;

namespace ToDoList.Api.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email {  get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [StringLength(100)]
        public string Password {  get; set; } = string.Empty;
    }
}
