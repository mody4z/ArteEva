namespace ArteEva.Models
{
    public class UserRole
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
