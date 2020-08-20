using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    public class CourseUpdateDto
    {
        [StringLength(1500)]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }
    }
}