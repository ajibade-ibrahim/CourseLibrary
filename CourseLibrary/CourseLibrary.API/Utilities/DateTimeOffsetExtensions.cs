using System;

namespace CourseLibrary.API.Utilities
{
    public static class DateTimeOffsetExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset)
        {
            var dateTimeNow = DateTime.UtcNow;

            if (dateTimeOffset.Date > dateTimeNow.Date)
            {
                throw new ArgumentException("Invalid argument. Provided date is in the future.");
            }

            var years = dateTimeNow.Year - dateTimeOffset.Year;
            return dateTimeOffset.AddYears(years) < dateTimeNow ? years - 1 : years;
        }
    }
}