using ArteEva.Data;
using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.Product;
using ArtEva.DTOs.ProductImage;
using ArtEva.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArtEva.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly IShopService _shopService;

        public ProductService(
            IProductRepository productRepository,
            IProductImageRepository productImageRepository,
            ICategoryRepository categoryRepository,
            ISubCategoryRepository subCategoryRepository,
            IShopService shopService)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _shopService = shopService;
        
        }

        public async Task<CreatedProductDto> CreateProductAsync(int userId, CreateProductDto dto)
        {
            // Validate input & business rules
            await ValidateProductCreationAsync(userId, dto);

            // Generate a unique SKU
            string sku = await GenerateUniqueSkuAsync(dto.ShopId, dto.CategoryId);

            // Build product entity
            var product = new Product
            {
                ShopId = dto.ShopId,
                CategoryId = dto.CategoryId,
                SubCategoryId = dto.SubCategoryId,
                Title = dto.Title,
                SKU = sku,
                Price = dto.Price,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChanges();

            // Save product images
            if (dto.Images != null && dto.Images.Any())
            {
                var images = dto.Images.Select(i => new ProductImage
                {
                    ProductId = product.Id,
                    Url = i.Url,
                    AltText = i.AltText,
                    SortOrder = i.SortOrder,
                    IsPrimary = i.IsPrimary,
                    CreatedAt = DateTime.UtcNow
                });

                await _productImageRepository.AddRangeAsync(images);
                await _productImageRepository.SaveChanges();
            }

            // Load complete product with images
            var loadedProduct = await _productRepository.GetProductWithImagesAsync(product.Id);

            return MapToProductDto(loadedProduct);
        }


        #region Private methods
        private async Task ValidateProductCreationAsync(int userId, CreateProductDto dto)
        {
            // 1. Validate shop
            var shop = await _shopService.GetShopByIdAsync(dto.ShopId);

            if (shop == null)
                throw new Exception("Shop not found.");

            if (shop.OwnerUserId != userId)
                throw new Exception("You are not the owner of this shop.");

            if (shop.Status != ShopStatus.Active)
                throw new Exception("Adding products is not allowed unless the shop is active.");

            // 2. Validate category exists
            var categoryExists = await _categoryRepository.AnyAsync(c =>
                c.Id == dto.CategoryId && !c.IsDeleted);

            if (!categoryExists)
                throw new Exception("Invalid category.");

            // 3. Validate subcategory ownership
            var subCategory = await _subCategoryRepository.FirstOrDefaultAsync(sc =>
                sc.Id == dto.SubCategoryId &&
                sc.CategoryId == dto.CategoryId &&
                !sc.IsDeleted);

            if (subCategory == null)
                throw new Exception("Invalid subcategory or does not belong to the selected category.");
        }
    

        private CreatedProductDto MapToProductDto(Product product)
        {
            return new CreatedProductDto
            {
                Id = product.Id,
                ShopId = product.ShopId,
                CategoryId = product.CategoryId,
                SubCategoryId = product.SubCategoryId,
                Title = product.Title,
                SKU = product.SKU,
                Price = product.Price,
                IsPublished = product.IsPublished,

                Images = product.ProductImages?
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new CreatedProductImageDto
                    {
                        Id = i.Id,
                        Url = i.Url,
                        AltText = i.AltText,
                        SortOrder = i.SortOrder,
                        IsPrimary = i.IsPrimary
                    })
                    .ToList()
            };
        }

        private string GenerateSku(int shopId, int categoryId)
        {
            // Example: SHP5-CAT12-AX93DK
            string random = Guid.NewGuid().ToString("N")[..6].ToUpper();
            return $"SHP{shopId}-CAT{categoryId}-{random}";
        }
        private async Task<string> GenerateUniqueSkuAsync(int shopId, int categoryId)
        {
            string sku;

            do
            {
                sku = GenerateSku(shopId, categoryId);

            } while (await _productRepository.AnyAsync(p =>
                p.SKU == sku &&
                p.ShopId == shopId &&
                !p.IsDeleted
            ));

            return sku;
        }
        #endregion

    }
}

