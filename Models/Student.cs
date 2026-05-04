using System.ComponentModel.DataAnnotations;

namespace StudentsBlazorApp.Models
{
    public class Student
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public int? Age { get; set; }
    }
}
