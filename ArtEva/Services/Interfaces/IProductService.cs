using ArteEva.Models;
using ArtEva.Application.Products.Quiries;
using ArtEva.DTOs.Pagination;
using ArtEva.DTOs.Pagination.Product;
using ArtEva.DTOs.Product;
using ArtEva.DTOs.ProductImage;
using ArtEva.Models.Enums;
using System.Linq.Expressions;

namespace ArtEva.Services
{
    public interface IProductService
    {
        public Task<CreatedProductDto> CreateProductAsync( CreateProductDto dto);
        Task UpdateProductBaseInfoAsync(Product product);
        Task UpdateProductPriceInternalAsync(Product product, decimal newPrice);
        Task UpdateProductStatusInternalAsync(Product product, ProductStatus status);
        Task DeleteProductAsync(int productId);

        Task<CreatedProductDto> GetProductByIdAsync(int productId);
        Task<Product> GetProductForUpdateAsync(int productId);

        // master dynamic paging method
        Task<PagedResult<ProductListItemDto>> GetProductsAsync(
         ProductQueryCriteria criteria,int pageNumber,int pageSize);

        Task<PagedResult<ProductCardDto>> GetProductCardsAsync(
         PublicProductQueryCriteria criteria, int pageNumber, int pageSize);
        Task<ApprovedProductDto> ApproveProductAsync(int productId);
        Task<RejectedProductDto> RejectProductAsync(ProductToReject dto);

        Task UpdateProductImagesAsync(Product product, List<UpdateProductImage> imagesDto);

    }
}
