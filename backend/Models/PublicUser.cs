using backend.Models.Preferences;

namespace backend.Models
{
    public class PublicUser
    {
        public string Id { get; set; }
        public string? Picture { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateOnly Birthdate { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public List<SelectionObject> Preferences { get; set; } = new();
        public List<SelectionObject> Restrictions { get; set; } = new();
    }
}
