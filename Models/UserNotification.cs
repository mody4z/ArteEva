using System;

namespace ArteEva.Models
{
    public class UserNotification
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int NotificationId { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Notification Notification { get; set; }
    }
}
