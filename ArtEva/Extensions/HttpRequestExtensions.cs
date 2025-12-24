using ArteEva.Models;
using ArtEva.DTOs.Pagination;
using ArtEva.DTOs.Pagination.Product;
using ArtEva.DTOs.Product;
using Microsoft.AspNetCore.Http;
namespace ArtEva.Extensions
{
   
    public static class HttpRequestExtensions
    {
        public static string BuildPublicUrl(this HttpRequest request, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{relativePath.TrimStart('/')}";
        }
     
        public static void BuildProductImagesUrls(this HttpRequest request,IEnumerable<ProductWithImagesDto>? products)
        {
            if (products == null) return;
            foreach (var product in products)
            {
                if (product.Images == null) continue;

                foreach (var image in product.Images)
                {
                    image.Url = request.BuildPublicUrl(image.Url);
                }
            }
        }

        public static void BuildPagedProductCardImagesUrls(this HttpRequest request, PagedResult<ProductCardDto> products)
        {
            if (products == null) return;
            IEnumerable<ProductCardDto> productCards = products.Items;
            foreach (var productCard in productCards)
            {
                if (productCard.Images == null) continue;

                foreach (var image in productCard.Images)
                {
                    image.Url = request.BuildPublicUrl(image.Url);
                }
            }
        }
        public static void BuildCreatedProductImagesUrls(this HttpRequest request, CreatedProductDto product)
        {
            if (product is null || product.Images is null) return;
            foreach (var image in product.Images)
            {
                image.Url = request.BuildPublicUrl(image.Url);
            }
        }
    }
 
}
