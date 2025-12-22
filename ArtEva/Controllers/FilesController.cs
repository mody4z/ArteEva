using ArtEva.DTOs;
using ArtEva.Services;
using ArtEva.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtEva.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Seller")]

    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] FileUploadDto dto, [FromQuery] string type = "general")
        {
            var file = dto.File;
            var url = await _fileService.UploadImageAsync(file, type);
            return Ok(new { imageUrl = url });
        }

    }
}
