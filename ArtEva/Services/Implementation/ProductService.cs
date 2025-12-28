using ArteEva.Data;
using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.Application.Products.Quiries;
using ArtEva.Application.Products.Specifications;
using ArtEva.DTOs.Pagination;
using ArtEva.DTOs.Pagination.Product;
using ArtEva.DTOs.Product;
using ArtEva.DTOs.ProductImage;
using ArtEva.DTOs.Shop;
using ArtEva.Models.Enums;
using ArtEva.Repositories.Interfaces;
using ArtEva.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace ArtEva.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        public ProductService(
            IUnitOfWork unitOfWork,
            IConfiguration config
            )
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<CreatedProductDto> CreateProductAsync( CreateProductDto dto)
        {
            // Generate a unique SKU
            var sku = await GenerateUniqueSkuAsync(dto.ShopId, dto.CategoryId);

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

            await _unitOfWork.ProductRepository.AddAsync(product);

            if (dto.Images != null && dto.Images.Any())
            {
                var images = dto.Images.Select(i => new ProductImage
                {
                    Product = product, // no ProductId yet, EF handles it
                    Url = i.Url,
                    AltText = i.AltText,
                    SortOrder = i.SortOrder,
                    IsPrimary = i.IsPrimary,
                    CreatedAt = DateTime.UtcNow
                });

                await _unitOfWork.ProductImageRepository.AddRangeAsync(images);
            }

            await _unitOfWork.SaveChangesAsync(); 

            var loadedProduct =
                await _unitOfWork.ProductRepository.GetProductWithImagesAsync(product.Id);

            return MapToProductDto(loadedProduct);
        }

        ////////////////////////////////////////////////////////////
        ///Get Actions
        /////////////////////////////////////////////////////////
        public async Task<CreatedProductDto> GetProductByIdAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetProductWithImagesAsync(productId);
            if (product == null || product.IsDeleted)
                throw new KeyNotFoundException("Product not found.");
            var existedProduct = MapToProductDto(product);

            return existedProduct; 
        }

        public async Task<Product> GetProductForUpdateAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIDWithTrackingAsync(productId);

            if (product == null || product.IsDeleted)
                throw new NotFoundException("Product not found.");

            return product;
        }
        #region Paged products 
       
        // Master dynamic paging method
        public async Task<PagedResult<ProductListItemDto>> GetProductsAsync(
        ProductQueryCriteria criteria,
        int pageNumber,
        int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);

            var specification = new ProductQuerySpecification(criteria);

            var items = await _unitOfWork.ProductRepository.GetPagedAsync(
                specification,
                pageNumber,
                pageSize);

            var total = await _unitOfWork.ProductRepository.CountAsync(specification);

            return new PagedResult<ProductListItemDto>
            {
                Items = items.Select(MapToListItemDto).ToList(),
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<ProductCardDto>> GetProductCardsAsync(
     PublicProductQueryCriteria criteria,
     int pageNumber,
     int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);

            var specification = new PublicProductQuerySpecification(criteria);

            var products = await _unitOfWork.ProductRepository.GetPagedAsync(
                specification,
                pageNumber,
                pageSize);

            var productCards = products.Select(p => new ProductCardDto
            {
                ProductId = p.Id,
                ProductTitle = p.Title,
                productPrice = p.Price,
                Images = p.ProductImages
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        Url = BuildProductImageUrl(i.Url),
                        AltText = i.AltText,
                        SortOrder = i.SortOrder,
                        IsPrimary = i.IsPrimary
                    })
                    .ToList()
            }).ToList();

            var totalCount = await _unitOfWork.ProductRepository.CountAsync(specification);

            return new PagedResult<ProductCardDto>
            {
                Items = productCards,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        #endregion

        #region Update Product
        // UPDATE PRODUCT
        public Task UpdateProductBaseInfoAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            product.ApprovalStatus = ProductApprovalStatus.Pending;
            return Task.CompletedTask;
        }

        public Task UpdateProductStatusInternalAsync(Product product, ProductStatus status)
        {
            if (!Enum.IsDefined(typeof(ProductStatus), status))
                throw new NotValidException("Invalid product status.");

            product.Status = status;
            product.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task UpdateProductPriceInternalAsync(Product product, decimal newPrice)
        {
            if (newPrice <= 0)
                throw new NotValidException("Price must be greater than zero.");

            product.Price = newPrice;
            product.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        #endregion

        #region Delete Product
        public async Task DeleteProductAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIDWithTrackingAsync(productId);

            if (product == null || product.IsDeleted)
                throw new NotFoundException("Product not found.");

            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
        }
        #endregion
        // Admin Actions
        #region Admin Actions
        public async Task<ApprovedProductDto> ApproveProductAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIDWithTrackingAsync(productId);

            if (product == null)
                throw new NotFoundException($"Product not found.");

            product.IsPublished = true;
            product.ApprovalStatus = ProductApprovalStatus.Approved;
            
            await _unitOfWork.SaveChangesAsync();
            var approved = new ApprovedProductDto
            {
                Id = productId,
                Title = product.Title,
                SKU = product.SKU,
                IsPublished = product.IsPublished,
                ApprovalMessage = $"Product with Title: {product.Title} is published now"
            };

            return approved;
        }

        public async Task<RejectedProductDto> RejectProductAsync(ProductToReject dto)
        {
            var product = await _unitOfWork.ProductRepository.GetByIDWithTrackingAsync(dto.ProductId);

            if (product == null)
                throw new NotFoundException($"Product this product not found.");

            product.IsPublished = false;
            product.ApprovalStatus = ProductApprovalStatus.Rejected;
            product.RejectionMessage = dto.RejectionMessage;

            await _unitOfWork.SaveChangesAsync();

            var rejProduct = await _unitOfWork.ProductRepository.GetByIdAsync(product.Id);
            var rejected = new RejectedProductDto
            {
                Id = rejProduct.Id,
                Title = rejProduct.Title,
                SKU = rejProduct.SKU,
                IsPublished = rejProduct.IsPublished,
                RejectionMessage = rejProduct.RejectionMessage
            };

            return rejected;
        }

        #endregion


        #region Private methods

        //Image Helper
        #region ImageHelper
        private string BuildProductImageUrl(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            var baseUrl = _config["UploadSettings:BaseUrl"];
            return $"/uploads/products/{fileName}";
        }

        #endregion
        #region Mapping
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
                                 Url = BuildProductImageUrl(i.Url),
                                 AltText = i.AltText,
                                 SortOrder = i.SortOrder,
                                 IsPrimary = i.IsPrimary
                              })
                                .ToList()
            };
        }

        private ProductListItemDto MapToListItemDto(Product p)
        {
            return new ProductListItemDto
            {
                Id = p.Id,
                ShopId = p.ShopId,
                Title = p.Title,
                SKU = p.SKU,
                Price = p.Price,
                Status = p.Status,
                IsPublished = p.IsPublished,
                Images = p.ProductImages?
                          .OrderBy(i => i.SortOrder)
                          .Select(i => new ProductImageDto
                                           {
                                                Id = i.Id,
                                                Url = BuildProductImageUrl(i.Url),
                                                AltText = i.AltText,
                                                SortOrder = i.SortOrder,
                                                IsPrimary = i.IsPrimary
                                            }).ToList() ?? new List<ProductImageDto>()
            };
        }

        #endregion
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

            } while (await _unitOfWork.ProductRepository.AnyAsync(p =>
                p.SKU == sku &&
                p.ShopId == shopId &&
                !p.IsDeleted
            ));

            return sku;
        }

        #region image helper
        public async Task UpdateProductImagesAsync(
                Product product,List<UpdateProductImage> imagesDto)
        {
            var existing =
                await _unitOfWork.ProductImageRepository
                    .GetImagesByProductIdWithTracking(product.Id);

            // delete
            var incomingIds = imagesDto.Where(i => i.Id != 0).Select(i => i.Id).ToList();
            var toDelete = existing.Where(e => !incomingIds.Contains(e.Id)).ToList();
            if (toDelete.Any())
                _unitOfWork.ProductImageRepository.RemoveRange(toDelete);

            // update
            foreach (var incomingImg in imagesDto.Where(i => i.Id != 0))
            {
                var entity = existing.First(i => i.Id == incomingImg.Id);
                entity.Url = incomingImg.Url;
                entity.AltText = incomingImg.AltText;
                entity.SortOrder = incomingImg.SortOrder;
                entity.IsPrimary = incomingImg.IsPrimary;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            // add
            var newImages = imagesDto
                .Where(i => i.Id == 0)
                .Select(i => new ProductImage
                {
                    ProductId = product.Id,
                    Url = i.Url,
                    AltText = i.AltText,
                    SortOrder = i.SortOrder,
                    IsPrimary = i.IsPrimary,
                    CreatedAt = DateTime.UtcNow
                });

            await _unitOfWork.ProductImageRepository.AddRangeAsync(newImages);
        }


        #endregion

        #endregion

    }
}

