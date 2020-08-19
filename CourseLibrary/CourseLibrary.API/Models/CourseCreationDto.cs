using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    [ProhibitedWordsFilter]
    public class CourseCreationDto : IValidatableObject
    {
        [Required]
        [StringLength(1500)]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title.Equals(Description))
            {
                yield return new ValidationResult(
                    "The title should be different from the description.",
                    new[]
                    {
                        "Course"
                    });
            }
        }
    }
}