using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityForum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Received GET request to retrieve all categories");
            return Ok(await _categoryService.GetAllCategoriesAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("Received GET request to retrieve category by ID {id}", id);
            return Ok(await _categoryService.GetCategoryByIdAsync(id));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            _logger.LogInformation("Received POST request to create new category");
            return Ok(await _categoryService.CreateCategoryAsync(request));
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(UpdateCategoryRequest request)
        {
            _logger.LogInformation("Received PUT request to update category by ID {id}", request.Id);
            return Ok(await _categoryService.UpdateCategoryAsync(request));
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(DeleteCategoryRequest request)
        {
            _logger.LogInformation("Received DELETE request to delete category by ID {id}", request.Id);
            await _categoryService.DeleteCategoryAsync(request);
            return Ok();
        }
    }
}
