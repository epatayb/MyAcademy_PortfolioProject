namespace Portfolio.Data.Entities
{
    public class Education
    {
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public double GPA { get; set; }
        public int StartYear { get; set; }
        public string? GraduationYear { get; set; }        
    }
}
