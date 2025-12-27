using System.ComponentModel.DataAnnotations;

namespace ArtEva.DTOs.CartDTOs
{
    /// <summary>
    /// Request to update item quantity.
    /// </summary>
    public class UpdateItemRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}