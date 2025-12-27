using System.ComponentModel.DataAnnotations;

namespace ArtEva.DTOs. CartItem
{
    /// <summary>
    /// Request to add an item to cart.
    /// </summary>
    public class AddItemRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public int Quantity { get; set; }
    }}