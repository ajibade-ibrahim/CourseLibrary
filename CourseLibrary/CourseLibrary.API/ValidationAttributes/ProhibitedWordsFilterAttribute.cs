using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.ValidationAttributes
{
    public class ProhibitedWordsFilterAttribute : ValidationAttribute
    {
        private readonly List<string> _prohibitedWords = new List<string>
        {
            "crap",
            "fudge",
            "hell",
            "dive"
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Invalid data entered.");
            }

            var courseCreationDto = (CourseCreationDto)value;
            var inputWords = string.Concat(courseCreationDto.Title, courseCreationDto.Description).Split(" ");

            var containedWords = _prohibitedWords.Where(word => inputWords.Any(input => word.Equals(input, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (containedWords.Any())
            {
                return new ValidationResult(
                    $"Prohibited words found in title/description: {string.Join(',', containedWords)}",
                    new[]
                    {
                        "Course"
                    });
            }

            return ValidationResult.Success;
        }
    }
}