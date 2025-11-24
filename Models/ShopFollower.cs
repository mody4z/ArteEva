using System;

namespace ArteEva.Models
{
    public class ShopFollower
    {
        public int Id { get; set; }

        public int ShopId { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public Shop Shop { get; set; }
        public User User { get; set; }
    }
}
