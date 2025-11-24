using System;

namespace ArteEva.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ProductId { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Product Product { get; set; }
    }
}
