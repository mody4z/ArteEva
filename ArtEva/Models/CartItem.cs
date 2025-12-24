namespace ArteEva.Models
{
    public class CartItem : BaseModel
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal price { get; set; }

        // Navigation Properties
        public Cart Cart { get; set; }
        public Product Product { get; set; }
        public User User { get; set; }
    }
}
