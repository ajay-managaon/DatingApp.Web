namespace Web.DatingApp.API.Web.DatingApp.Extensions
{
    public static class CalculateAgeExtensionMethod
    {
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            // Save today's date.
            var today = DateTime.Today;

            // Calculate the age.
            var age = today.Year - dateOfBirth.Year;

            // Go back to the year in which the person was born in case of a leap year
            if (dateOfBirth.Date > today.AddYears(-age)) age--;

            return age;
        }
    }
}
