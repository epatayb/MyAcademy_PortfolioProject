using System.ComponentModel.DataAnnotations;

namespace Portfolio.ViewModels
{
    public class TechStackCreateViewModel
    {
        [Display(Name = "Teknoloji Adı")]
        [Required(ErrorMessage = "Teknoloji adı boş bırakılamaz.")]
        [StringLength(30, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir.")]
        public string Name { get; set; } = string.Empty;
    }
}
