using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseLibrary.Domain
{
    public class Course
    {
        [ForeignKey("AuthorId")]
        public Author Author { get; set; }

        public Guid AuthorId { get; set; }

        [MaxLength(1500)]
        public string Description { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
    }
}