namespace ArtEva.DTOs.CartItem
{
    public class UpdateCartitemDTO
    {
        public int ID { get; set; }
        public int CartID { get; set; }

        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}
