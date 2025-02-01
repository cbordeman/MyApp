using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TagAuc.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [Required]
    [StringLength(30, ErrorMessage = "{2}-{1} chars.", MinimumLength = 2)]
    [DataType(DataType.Text)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = "";

    [Required]
    [StringLength(30, ErrorMessage = "{2}-{1} chars.", MinimumLength = 2)]
    [DataType(DataType.Text)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = "";

    [StringLength(6, ErrorMessage = "Max {1} chars")]
    [DataType(DataType.Text)]
    [Display(Name = "Suffix")]
    public string Suffix { get; set; } = "";

    // [StringLength(50, ErrorMessage = "Required", MinimumLength = 1)]
    // [DataType(DataType.Text)]
    // [Display(Name = "Title or Position")]
    // public string Title { get; set; } = "";

    [Required]
    [StringLength(50, ErrorMessage = "Max chars: {0}.")]
    [DataType(DataType.Text)]
    [Display(Name = "Dealership")]
    public string Dealership { get; set; } = "";

    [Required]
    [StringLength(12, ErrorMessage = "Must be {0} chars.")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\d{3}-\d{3}-\d{4}", ErrorMessage = "Use ###-###-#### format.")]
    [Display(Name = "Phone Number")]
    public override string? PhoneNumber { get; set; } = "";

    [StringLength(50, ErrorMessage = "Max chars: {1}.")]
    [DataType(DataType.Text)]
    [Display(Name = "Extension")]
    public string PhoneExtension { get; set; } = "";

    //[Required]
    //[StringLength(50, ErrorMessage = "Max chars: {0}.")]
    //[DataType(DataType.Text)]
    //[Display(Name = "Dealership From Registration Form (verify)")]
    //public string DealershipFromRegistrationForm { get; set; } = "";

    // Must fill out if a Dealership user
    public int DealerId { get; set; }
}