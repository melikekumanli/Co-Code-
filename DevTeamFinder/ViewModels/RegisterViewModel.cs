using System.ComponentModel.DataAnnotations;

namespace DevTeamFinder.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
    [Display(Name = "Ad Soyad")]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre alanı zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [Display(Name = "Şifre")]
    [DataType(DataType.Password)]
    public string Sifre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre Tekrar alanı zorunludur.")]
    [Compare("Sifre", ErrorMessage = "Şifreler eşleşmiyor.")]
    [Display(Name = "Şifre Tekrar")]
    [DataType(DataType.Password)]
    public string SifreTekrar { get; set; } = string.Empty;
}
