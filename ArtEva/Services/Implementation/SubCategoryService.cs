using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.Category;
using ArtEva.DTOs.subCategory;
using ArtEva.Repositories.Interfaces;
using ArtEva.Services.Implementation;
using ArtEva.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ArtEva.Services
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService categoryService;
        public SubCategoryService(IUnitOfWork unitOfWork, ICategoryService categoryService)
        {
            _unitOfWork = unitOfWork;
            this.categoryService = categoryService;
        }

        public  async Task<IEnumerable<SubCategoryDTO>> GetallSubCategoryByCategoryIdAsync(int CategoryId)
        {
            bool Exits =await categoryService.ValidateCategoryExistsAsync(CategoryId);
            if (!Exits)
            {
                throw new NotFoundException("Category not found");

            }
            var Sub =await _unitOfWork.SubCategoryRepository
                .GetAllAsync()
                .Where(sub => sub.CategoryId == CategoryId)
                .Select(sub=> new SubCategoryDTO()
                {
                    Id=sub.Id,  
                    Name =sub.Name,

                }).ToListAsync();
            return Sub;
        }

        public async Task<SubCategoryDTO> CreateSubCategoryAsync(CreateSubCategory req)
        {
            var existingSubCategory = await _unitOfWork.SubCategoryRepository.FirstOrDefaultAsync(c => c.Name == req.Name); ;

            if (existingSubCategory == null) {
                _unitOfWork.SubCategoryRepository.AddAsync(new ArteEva.Models.SubCategory { Name = req.Name, CategoryId = req.CategoryId.Value });
 
                await _unitOfWork.SaveChangesAsync();
                return new SubCategoryDTO
                {
                    
                    Name = req.Name,

                };
            }
            throw new NotValidException("Category with the same name already exists");



        }

        public async Task DeleteSubCategoryAsync(int id)
        {
            var existingSubCategory =  await _unitOfWork.SubCategoryRepository.GetByIdAsync(id);
            if (existingSubCategory == null)
            {
                throw new Exception("SubCategory not found");
            }
         await _unitOfWork.SubCategoryRepository.Delete(id);
         await _unitOfWork.SaveChangesAsync();



        }

        public async Task<IEnumerable<SubCategoryDTO>> GetAllSubCategoriesAsync()
        {
            var subCategories =   _unitOfWork.SubCategoryRepository.GetAllAsync();
            return subCategories.Select(c => new SubCategoryDTO
            {
                Id= c.Id,   
                Name = c.Name,
            }).ToList();    
        }

        public async Task<SubCategoryDTO> GetSubCategoryByIdAsync(int id)
        {
            var subCategory =await  _unitOfWork.SubCategoryRepository.GetByIdAsync(id);
            if (subCategory == null)
            {
                throw new Exception("SubCategory not found");
            
            }
            return new SubCategoryDTO
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
            };
        }

        public async Task<SubCategoryDTO> UpdateSubCategoryAsync(UpdateSubcategory req)
        {          
                   
            var existingSubCategory = await _unitOfWork.SubCategoryRepository.GetByIdAsync(req.Id);
            if (existingSubCategory == null)
            {
                throw new Exception("SubCategory not found");
            }
           var category = await categoryService.GetCategoryByIdAsync(req.CategoryId.Value);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            existingSubCategory.Name = req.Name ?? existingSubCategory.Name;
            existingSubCategory.CategoryId = req.CategoryId ?? existingSubCategory.CategoryId;
           await _unitOfWork.SubCategoryRepository.UpdateAsync(existingSubCategory);
            await _unitOfWork.SaveChangesAsync();
            return new SubCategoryDTO
            {
                Id= existingSubCategory.Id, 
                Name = existingSubCategory.Name
            };

        }

        //added
        public async Task<bool> ValidateSubCategoryAsync(int subCategoryId, int categoryId)
        {
            var exists = await _unitOfWork.SubCategoryRepository.AnyAsync(sc =>
                sc.Id == subCategoryId &&
                sc.CategoryId == categoryId &&
                !sc.IsDeleted);
            return exists;
         
        }
    }
}
