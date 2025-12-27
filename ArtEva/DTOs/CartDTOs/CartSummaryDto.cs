namespace ArtEva.DTOs.CartDTOs
{
    /// <summary>
    /// Lightweight cart summary (for header display).
    /// </summary>
    public class CartSummaryDto
    {
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}