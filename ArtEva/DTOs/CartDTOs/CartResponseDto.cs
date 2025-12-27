using ArtEva.DTOs.CartItem;

namespace ArtEva.DTOs.CartDTOs
{
    public class CartResponseDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
