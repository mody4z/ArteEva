using ArtEva.Application.Products.Quiries;
using ArtEva.Extensions;
using ArtEva.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArtEva.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            Request.BuildCreatedProductImagesUrls(product);
            return Ok(product);
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetPublicProducts(
            [FromQuery] PublicProductQueryCriteria query,
            int pageNumber = 1,
            int pageSize = 20)
        {
            var result = await _productService.GetProductCardsAsync(query,
                pageNumber,pageSize);
            Request.BuildPagedProductCardImagesUrls(result);
            return Ok(result);
        }
    }
}
