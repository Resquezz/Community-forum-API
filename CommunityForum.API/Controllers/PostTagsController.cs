using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityForum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostTagsController : ControllerBase
    {
        private readonly IPostTagService _postTagService;
        private readonly ILogger<PostTagsController> _logger;

        public PostTagsController(IPostTagService postTagService, ILogger<PostTagsController> logger)
        {
            _postTagService = postTagService;
            _logger = logger;
        }

        [HttpGet("post/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTagsByPostId(Guid postId)
        {
            _logger.LogInformation("Received GET request to retrieve tags for post ID {postId}", postId);
            return Ok(await _postTagService.GetTagsByPostIdAsync(postId));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreatePostTagRequest request)
        {
            _logger.LogInformation("Received POST request to create post-tag relation");
            return Ok(await _postTagService.AddPostTagAsync(request));
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(DeletePostTagRequest request)
        {
            _logger.LogInformation("Received DELETE request to delete post-tag relation. Post ID {postId}, Tag ID {tagId}",
                request.PostId, request.TagId);
            await _postTagService.DeletePostTagAsync(request);
            return Ok();
        }
    }
}
