using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.Domain
{
    public class Author
    {
        public ICollection<Course> Courses { get; set; } = new List<Course>();

        [Required]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string MainCategory { get; set; }
    }
}