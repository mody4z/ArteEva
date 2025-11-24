using System;
using System.ComponentModel.DataAnnotations;

namespace ArteEva.Models
{
    public enum DisputeStatus
    {
        Open,
        UnderReview,
        Resolved,
        Closed,
        Escalated
    }

    public class Dispute
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public int UserId { get; set; }

        public DisputeStatus Status { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public Order Order { get; set; }
        public User User { get; set; }
    }
}
