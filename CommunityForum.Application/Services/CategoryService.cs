using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Application.Mappers;
using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Interfaces;
using CommunityForum.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHubContext<ForumHub> _hubContext;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, IHubContext<ForumHub> hubContext, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to create category with null request instance.");
                throw new ArgumentNullException(nameof(request), "Create category request can not be null.");
            }

            var normalizedName = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                _logger.LogError("Attempt to create category without name.");
                throw new ArgumentException("Category name is required.", nameof(request.Name));
            }
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                _logger.LogError("Attempt to create category without description.");
                throw new ArgumentException("Category description is required.", nameof(request.Description));
            }

            var existingCategory = await _categoryRepository.GetByNameAsync(normalizedName);
            if (existingCategory != null)
            {
                _logger.LogError("Attempt to create category with duplicate name: {categoryName}", normalizedName);
                throw new InvalidOperationException($"Category '{normalizedName}' already exists.");
            }

            var category = new Category(normalizedName, request.Description.Trim());
            await _categoryRepository.AddAsync(category);

            var response = category.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.CategoryCreated.ToString(), response);
            _logger.LogInformation("Category created successfully.");
            return response;
        }

        public async Task DeleteCategoryAsync(DeleteCategoryRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to delete category with null request instance.");
                throw new ArgumentNullException(nameof(request), "Delete category request can not be null.");
            }

            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                _logger.LogError("Attempt to delete non existing category. Category id: {categoryId}", request.Id);
                throw new KeyNotFoundException($"Category with id {request.Id} not found.");
            }

            await _categoryRepository.DeleteAsync(request.Id);
            await _hubContext.Clients.All.SendAsync(EventType.CategoryDeleted.ToString(), new { request.Id });
            _logger.LogInformation("Category deleted successfully.");
        }

        public async Task<ICollection<CategoryResponseDTO>?> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            if (categories == null)
            {
                _logger.LogInformation("No categories to fetch from database.");
                return null;
            }

            _logger.LogInformation("Successfully fetched {count} categories from database.", categories.Count);
            return categories.Select(category => category.ToResponse()).ToList();
        }

        public async Task<CategoryResponseDTO> GetCategoryByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Attempt to retrieve category with empty id.");
                throw new ArgumentException("Category id can not be empty.", nameof(id));
            }

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogError("Attempt to retrieve non existing category. Category id: {categoryId}", id);
                throw new KeyNotFoundException($"Category with id {id} not found.");
            }

            _logger.LogInformation("Category with id {id} retrieved successfully.", id);
            return category.ToResponse();
        }

        public async Task<CategoryResponseDTO> UpdateCategoryAsync(UpdateCategoryRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to update category with null request instance.");
                throw new ArgumentNullException(nameof(request), "Update category request can not be null.");
            }

            var normalizedName = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                _logger.LogError("Attempt to update category name with empty. Category id: {categoryId}", request.Id);
                throw new ArgumentException("Category name is required.", nameof(request.Name));
            }
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                _logger.LogError("Attempt to update category description with empty. Category id: {categoryId}", request.Id);
                throw new ArgumentException("Category description is required.", nameof(request.Description));
            }

            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                _logger.LogError("Attempt to update non existing category. Category id: {categoryId}", request.Id);
                throw new KeyNotFoundException($"Category with id {request.Id} not found.");
            }

            var duplicateCategory = await _categoryRepository.GetByNameAsync(normalizedName);
            if (duplicateCategory != null && duplicateCategory.Id != category.Id)
            {
                _logger.LogError("Attempt to update category with duplicate name: {categoryName}", normalizedName);
                throw new InvalidOperationException($"Category '{normalizedName}' already exists.");
            }

            category.Name = normalizedName;
            category.Description = request.Description.Trim();
            await _categoryRepository.UpdateAsync(category);

            var response = category.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.CategoryUpdated.ToString(), response);
            _logger.LogInformation("Category updated successfully.");
            return response;
        }
    }
}
