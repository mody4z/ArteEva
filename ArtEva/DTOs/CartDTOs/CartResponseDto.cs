namespace ArtEva.DTOs.CartDTOs
{
    public class CartResponseDto
    {
        public int CartId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
