namespace backend.Models.Restrictions
{
    public class Restriction
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<UserRestriction> UserRestrictions { get; set; }
    }
}
