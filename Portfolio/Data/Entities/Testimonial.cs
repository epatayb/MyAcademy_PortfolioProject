namespace Portfolio.Data.Entities
{
    public class Testimonial
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Initails { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
    }
}