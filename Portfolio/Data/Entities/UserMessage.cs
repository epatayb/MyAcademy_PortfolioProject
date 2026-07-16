namespace Portfolio.Data.Entities
{
    public class UserMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MessageBody { get; set; }
        public bool IsRead { get; set; }
    }
}
