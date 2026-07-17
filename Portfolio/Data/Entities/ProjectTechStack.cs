namespace Portfolio.Data.Entities
{
    public class ProjectTechStack
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int TechStackId { get; set; }

        // navigation properties
        public Project Project { get; set; } = null!;
        public TechStack TechStack { get; set; } = null!;
    }
}
