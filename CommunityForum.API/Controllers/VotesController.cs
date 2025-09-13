using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CommunityForum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly IVoteService _voteService;
        private readonly ILogger<VotesController> _logger;
        public VotesController(IVoteService voteService, ILogger<VotesController> logger)
        {
            _voteService = voteService;
            _logger = logger;
        }

        // GET: api/<VotesController>
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    return _voteService.get
        //}

        // GET api/<VotesController>/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Received GET request to get vote by id {id}");
            return Ok(await _voteService.GetVoteByIdAsync(id));
        }

        // POST api/<VotesController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateVoteRequest request)
        {
            _logger.LogInformation($"Received POST request to create new vote");
            return Ok(await _voteService.CreateVoteAsync(request));
        }

        // PUT api/<VotesController>/
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(UpdateVoteRequest request)
        {
            _logger.LogInformation($"Received PUT request to update vote by id {request.Id}");
            return Ok(await _voteService.UpdateVoteAsync(request));
        }

        // DELETE api/<VotesController>/
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(DeleteVoteRequest request)
        {
            _logger.LogInformation($"Received DELETE request to delete vote by id {request.Id}");
            await _voteService.DeleteVoteAsync(request);
            return Ok();
        }
    }
}
