namespace backend.Models
{
    public class PublicUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateOnly Birthdate { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
    }
}
