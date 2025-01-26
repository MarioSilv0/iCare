namespace backend.Models.Restrictions
{
    public class UserRestriction
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public int RestrictionId { get; set; }
        public Restriction Restriction { get; set; }
    }
}
