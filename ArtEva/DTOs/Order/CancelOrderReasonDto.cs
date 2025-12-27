namespace ArtEva.DTOs.Order
{
    public class CancelOrderReasonDto
    {
        public string Reason { get; set; } = string.Empty;  
        public DateTime? CancelledAt { get; set; }
    }
}
