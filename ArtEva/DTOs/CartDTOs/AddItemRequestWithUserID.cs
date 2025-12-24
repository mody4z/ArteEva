namespace ArtEva.DTOs.CartDTOs
{
    public class AddItemRequestWithUserID
    {
        public int  UserID { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
