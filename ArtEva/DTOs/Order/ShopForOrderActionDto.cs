using ArtEva.Models.Enums;

namespace ArtEva.DTOs.Order
{
    public class ShopForOrderActionDto
    {
        public int ShopId { get; init; }
        public int OwnerUserId { get; init; }
        public ShopStatus Status { get; init; }
    }
}
