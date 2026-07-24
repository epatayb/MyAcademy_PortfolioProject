using System.ComponentModel.DataAnnotations;

namespace Portfolio.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-posta adresi boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; } = string.Empty;
    }
}