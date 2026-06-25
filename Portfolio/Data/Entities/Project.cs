namespace Portfolio.Data.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GithubUrl { get; set; }
        public List<ProjectTechStack> ProjectTechStacks { get; set; }
    }
}
