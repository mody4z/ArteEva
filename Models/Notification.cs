using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArteEva.Models
{
    public enum NotificationType
    {
        OrderUpdate,
        ProductUpdate,
        ShopUpdate,
        System,
        Promotion,
        Review
    }

    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public NotificationType Type { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public ICollection<UserNotification> UserNotifications { get; set; }
    }
}
