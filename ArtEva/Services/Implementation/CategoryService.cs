using ArteEva.Data;
using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.Category;
using ArtEva.Services.Interfaces;
using ArtEva.Services.Implementation;
using Azure.Core;
using ArtEva.Repositories.Interfaces;
namespace ArtEva.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork, IConfiguration config) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request)
        {

            var existingCategory = await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(c=>c.Name==request.Name);
            if(existingCategory != null)
            {
                throw new Exception("Category with the same name already exists");
            }
            Category category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ImageUrl = request.ImageUrl

            };
            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryDto
            {
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
            };  
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                throw new Exception("Category not found");

            }
            await _unitOfWork.CategoryRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var res=   _unitOfWork.CategoryRepository.GetAllAsync();
            return res.Select(c => new CategoryDto
            {
                Name = c.Name,
                Description = c.Description,
                ImageUrl = $"uploads/categories/{c.ImageUrl}",
            }).ToList();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                return null;


            return new CategoryDto
            {
                ID=existingCategory.Id,
                Name = existingCategory.Name,
                Description = existingCategory.Description,
                ImageUrl = $"uploads/categories/{existingCategory.ImageUrl}",
            };
              
        }

        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryRequestDto request)
        {

            var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
            if (existingCategory == null)
            {
                throw new Exception("Category not found");
            }
            existingCategory.Name = request.Name;
            existingCategory.Description = request.Description; 
            existingCategory.ImageUrl = request.ImageUrl;
            existingCategory.UpdatedAt = DateTime.UtcNow;   
           await _unitOfWork.CategoryRepository.UpdateAsync(existingCategory); 
            await _unitOfWork.SaveChangesAsync();
            return new CategoryDto
            {
                Name = existingCategory.Name,
                Description = existingCategory.Description,
                ImageUrl = existingCategory.ImageUrl,
            };
        }

        //added
        public async Task<bool> ValidateCategoryExistsAsync(int categoryId)
        {
            bool exists = await _unitOfWork.CategoryRepository.AnyAsync(c =>
                c.Id == categoryId && !c.IsDeleted);
            return exists;
        }

    }
}
