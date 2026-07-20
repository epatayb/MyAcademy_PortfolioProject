using System.ComponentModel.DataAnnotations;

namespace Portfolio.Data.Entities
{
    public class Education
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Okul veya kurum adı boş bırakılamaz.")]
        [MaxLength(100)]
        public string SchoolName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bölüm veya eğitim adı boş bırakılamaz.")]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama boş bırakılamaz.")]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Range(0, 4, ErrorMessage = "Not ortalaması 0 ile 4 arasında olmalıdır.")]
        public double? GPA { get; set; }

        [Required(ErrorMessage = "Başlangıç yılı boş bırakılamaz.")]
        [MaxLength(30)]
        public string StartYear { get; set; }

        [MaxLength(30)]
        public string? GraduationYear { get; set; }

        public bool IsCurrent { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
