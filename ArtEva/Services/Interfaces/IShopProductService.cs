using ArtEva.DTOs.Pagination;
using ArtEva.DTOs.Pagination.Product;
using ArtEva.DTOs.Product;
using ArtEva.DTOs.Shop;
using ArtEva.Models.Enums;

namespace ArtEva.Services.Interfaces
{
    public interface IShopProductService
    {
         Task<CreatedShopDto> GetShopByOwnerIdAsync(int userId, int pageNumber, int pageSize);
        //product section
        Task<CreatedProductDto> CreateShopProductAsync(int userId, CreateProductDto dto);
        Task<ArtEva.DTOs.Pagination.PagedResult<ProductListItemDto>> GetAllShopProductsAsync
            (int userId, int shopId, int pageNumber, int pageSize);
        Task<ArtEva.DTOs.Pagination.PagedResult<ProductListItemDto>> GetShopActiveProductsAsync(int userId,int shopId, int pageNumber, int pageSize);
        Task<ArtEva.DTOs.Pagination.PagedResult<ProductListItemDto>> GetShopInactiveProductsAsync(int userId,int shopId, int pageNumber, int pageSize);
        Task<CreatedProductDto> UpdateShopProductAsync(int userId, UpdateProductDto dto);
        Task<UpdatedProductStatusDto> UpdateProductStatusAsync(int userId, int shopId, int productId, ProductStatus status);
        Task<UpdatedProductPriceDto> UpdateProductPriceAsync(int userId, int shopId, int productId, decimal newPrice);
        Task DeleteShopProduct(int productId, int userId, int shopId);
    }
}
