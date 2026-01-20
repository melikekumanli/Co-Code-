using System.ComponentModel.DataAnnotations;

namespace DevTeamFinder.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre alanı zorunludur.")]
    [Display(Name = "Şifre")]
    [DataType(DataType.Password)]
    public string Sifre { get; set; } = string.Empty;
}
