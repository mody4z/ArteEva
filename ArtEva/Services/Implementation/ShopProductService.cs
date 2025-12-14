using ArteEva.Data;
using ArteEva.Repositories;
using ArtEva.DTOs.Shop;
using ArtEva.DTOs.Shop.Products;
using ArtEva.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtEva.Services.Implementation
{
    public class ShopProductService : IShopProductService
    {
        private readonly IShopService _shopService;
        private readonly IProductService _productService;
        private readonly IShopRepository _shopRepository;
        private readonly IConfiguration _config;

        public ShopProductService(
            IShopService shopService,
            IProductService productService
            ,IShopRepository shopRepository
            , IConfiguration config)
        {
            _shopService = shopService;
            _productService = productService;
            _shopRepository = shopRepository;
            _config = config;
        }
        public async Task<CreatedShopDto> GetShopByOwnerIdAsync(int userId, int pageNumber, int pageSize)
        {
            var baseUrl = _config["UploadSettings:BaseUrl"];
            CreatedShopDto? shop = 
              await _shopRepository.GetShopByOwnerId(userId)
             .Select(s => new CreatedShopDto
             {
                 Id = s.Id,
                 OwnerUserName = s.Owner.UserName,
                 Name = s.Name,
                 ImageUrl = $"{baseUrl}/uploads/shops/{s.ImageUrl}",
                 Description = s.Description,
                 Status = s.Status,
                 RatingAverage = s.RatingAverage,
                  
             }).FirstOrDefaultAsync();

            var res1 = await _productService.GetShopActiveProductsAsync(shop.Id, pageNumber, pageSize);
            var res2 = await _productService.GetShopInactiveProductsAsync(shop.Id, pageNumber, pageSize);

            var ActiveProductDtos = res1.Items.Select(s => new ActiveProductDto()
            {
                Title = s.Title,
                Images = s.Images,
                Price = s.Price,
                Status = s.Status,

            }).ToList();

            var InactiveProductDtos = res2.Items.Select(s => new InActiveProductDto()
            {
                Title = s.Title,
                Images = s.Images,
                Price = s.Price,
                Status = s.Status,

            }).ToList();
            shop.inActiveProductDtos = InactiveProductDtos;
            shop.activeProductDtos = ActiveProductDtos;
            if (shop == null)
            {
                return null;
            }

            return shop;
        }
    }
}
