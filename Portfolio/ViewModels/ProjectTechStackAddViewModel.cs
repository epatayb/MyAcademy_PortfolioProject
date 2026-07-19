using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.ViewModels
{
    public class ProjectTechStackAddViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bir teknoloj seçmelisiniz.")]
        public int? TechStackId { get; set; }

        public List<SelectListItem> AvailableTechStacks { get; set; } = [];
        public List<AddedTechStackViewModel> AddedTechStacks { get; set; } = [];

        public class AddedTechStackViewModel
        {
            public int ProjectTechStackId { get; set; }
            public int TechStackId { get; set; }
            public string Name { get; set; } = string.Empty;
            public int SortOrder { get; set; }
        }
    }
}
