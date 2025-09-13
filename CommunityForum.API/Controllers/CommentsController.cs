using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CommunityForum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;
        public CommentsController(ILogger<CommentsController> logger, ICommentService commentService)
        {
            _commentService = commentService;
            _logger = logger;
        }

        // GET: api/<CommentsController>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Received GET request to retrieve all comments");
            return Ok(await _commentService.GetAllCommentsAsync());
        }

        // GET api/<CommentsController>/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Received GET request to retrieve comment by ID {id}");
            return Ok(await _commentService.GetCommentByIdAsync(id));
        }

        // POST api/<CommentsController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateCommentRequest request)
        {
            _logger.LogInformation("Received POST request to create new comment");
            return Ok(await _commentService.CreateCommentAsync(request));
        }

        // PUT api/<CommentsController>/
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(UpdateCommentRequest request)
        {
            _logger.LogInformation($"Received PUT request to update comment with id {request.Id}");
            return Ok(await _commentService.UpdateCommentAsync(request));
        }

        // DELETE api/<CommentsController>/
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(DeleteCommentRequest request)
        {
            _logger.LogInformation($"Received DELETE request to delete comment with id {request.Id}");
            await _commentService.DeleteCommentAsync(request);
            return Ok();
        }
    }
}
