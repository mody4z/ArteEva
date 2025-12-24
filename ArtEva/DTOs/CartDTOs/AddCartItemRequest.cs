namespace ArtEva.DTOs.CartDTOs
{
    public class AddCartItemRequest
    {
         public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

}
