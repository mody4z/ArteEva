namespace ArtEva.DTOs.Order
{
    public class PricingResult
    {
        public decimal ShippingFee { get; init; }
        public decimal TaxTotal { get; init; }
        public decimal GrandTotal { get; init; }
    }
}
