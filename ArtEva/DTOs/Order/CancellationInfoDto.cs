namespace ArtEva.DTOs.Order
{
    public class CancellationInfoDto
    {
        public string Reason { get; set; } = string.Empty;  
        public DateTime? CancelledAt { get; set; }
        public int CancelledByUserId { get; set; }
    }
}
