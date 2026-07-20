using System.ComponentModel.DataAnnotations;

namespace Portfolio.Data.Entities
{
    public class Experience
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pozisyon adı boş bırakılamaz.")]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama boş bırakılamaz.")]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şirket adı boş bırakılamaz.")]
        [MaxLength(100)]
        public string Company { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlangıç yılı boş bırakılamaz.")]
        [MaxLength(30)]
        public string StartYear { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? EndYear { get; set; }

        public bool IsCurrent { get; set; }

        public bool IsActive { get; set; } = true;
    }
}