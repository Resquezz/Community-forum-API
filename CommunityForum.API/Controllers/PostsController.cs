using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CommunityForum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostsController> _logger;
        public PostsController(IPostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        // GET: api/<PostsController>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Received GET request to retrieve all posts");
            return Ok(await _postService.GetAllPostsAsync());
        }

        // GET api/<PostsController>/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Received GET request to retrieve comment by ID {id}");
            return Ok(await _postService.GetPostByIdAsync(id));
        }

        // POST api/<PostsController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreatePostRequest request)
        {
            _logger.LogInformation("Received POST request to create new comment");
            return Ok(await _postService.CreatePostAsync(request));
        }

        // PUT api/<PostsController>/
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(UpdatePostRequest request)
        {
            _logger.LogInformation($"Received PUT request to update comment by ID {request.Id}");
            return Ok(await _postService.UpdatePostAsync(request));
        }
        
        // DELETE api/<PostsController>/
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(DeletePostRequest request)
        {
            _logger.LogInformation($"Received DELETE request to delete comment by ID {request.Id}");
            await _postService.DeletePostAsync(request);
            return Ok();
        }
    }
}
