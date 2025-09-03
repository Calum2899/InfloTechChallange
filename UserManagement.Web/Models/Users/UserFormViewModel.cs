using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Users;

public class UserFormViewModel
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Forename is required")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Forename cannot contain numbers or special characters")]
    public string Forename { get; set; } = string.Empty;

    [Required(ErrorMessage = "Surname is required")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Surname cannot contain numbers or special characters")]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(UserFormViewModel), nameof(ValidateDateOfBirth))]
    public DateOnly DateOfBirth { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Please enter a valid email address (e.g. user@example.com)")]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public static ValidationResult? ValidateDateOfBirth(DateOnly dob, ValidationContext context)
    {
        if (dob > DateOnly.FromDateTime(DateTime.Today))
        {
            return new ValidationResult("Date of birth cannot be in the future");
        }
        return ValidationResult.Success;
    }
}
