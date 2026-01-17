using System.Text.RegularExpressions;

namespace Bus_ticketing_Backend.Services
{
    public class PasswordValidator
    {
        private const int MinLength = 8;
        private const string UppercasePattern = @"[A-Z]";
        private const string LowercasePattern = @"[a-z]";
        private const string DigitPattern = @"[0-9]";
        private const string SpecialCharPattern = @"[!@#$%^&*()_+\-=\[\]{};':""\|,.<>\/?]";

        public static (bool IsValid, string ErrorMessage) ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password cannot be empty.");

            if (password.Length < MinLength)
                return (false, $"Password must be at least {MinLength} characters long.");

            if (!Regex.IsMatch(password, UppercasePattern))
                return (false, "Password must contain at least one uppercase letter (A-Z).");

            if (!Regex.IsMatch(password, LowercasePattern))
                return (false, "Password must contain at least one lowercase letter (a-z).");

            if (!Regex.IsMatch(password, DigitPattern))
                return (false, "Password must contain at least one digit (0-9).");

            if (!Regex.IsMatch(password, SpecialCharPattern))
                return (false, "Password must contain at least one special character (!@#$%^&*).");

            return (true, string.Empty);
        }
    }
}