using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityForum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagsController> _logger;

        public TagsController(ITagService tagService, ILogger<TagsController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Received GET request to retrieve all tags");
            return Ok(await _tagService.GetAllTagsAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("Received GET request to retrieve tag by ID {id}", id);
            return Ok(await _tagService.GetTagByIdAsync(id));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateTagRequest request)
        {
            _logger.LogInformation("Received POST request to create new tag");
            return Ok(await _tagService.CreateTagAsync(request));
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(UpdateTagRequest request)
        {
            _logger.LogInformation("Received PUT request to update tag by ID {id}", request.Id);
            return Ok(await _tagService.UpdateTagAsync(request));
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(DeleteTagRequest request)
        {
            _logger.LogInformation("Received DELETE request to delete tag by ID {id}", request.Id);
            await _tagService.DeleteTagAsync(request);
            return Ok();
        }
    }
}
