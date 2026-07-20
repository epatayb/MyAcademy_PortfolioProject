namespace Portfolio.Data.Entities
{
    public class TechStack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public List<ProjectTechStack> ProjectTechStacks { get; set; }
    }
}
