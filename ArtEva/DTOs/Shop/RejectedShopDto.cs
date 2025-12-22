using ArtEva.Models.Enums;

namespace ArtEva.DTOs.Shop
{
    public class RejectedShopDto
    {
        public int ShopId { get; set; }     
        public string ShopName { get; set; }
        public string RejectionMessage { get; set; }
         public string ImageUrl { get; set; }
        public string Description { get; set; }
        public ShopStatus Status { get; set; }
     }
}
