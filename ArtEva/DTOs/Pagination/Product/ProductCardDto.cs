namespace ArtEva.DTOs.Pagination.Product
{
    public class ProductCardDto
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public decimal productPrice { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();

    }
}
