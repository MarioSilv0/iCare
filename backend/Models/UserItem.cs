namespace backend.Models
{
    public class UserItem
    {
        public string ItemName { get; set; }
        public float Quantity { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
