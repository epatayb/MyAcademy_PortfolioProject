using System.ComponentModel.DataAnnotations;

namespace Portfolio.ViewModels
{
    public class ResetPasswordViewModel
    {
        public int AdminId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre boş bırakılamaz.")]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage =
                "Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı boş bırakılamaz.")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(NewPassword),
            ErrorMessage = "Şifreler birbiriyle uyuşmuyor.")]
        [Display(Name = "Yeni Şifre Tekrarı")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}