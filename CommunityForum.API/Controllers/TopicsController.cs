using Azure.Core;
using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CommunityForum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly ILogger<TopicsController> _logger;
        public TopicsController(ITopicService topicService, ILogger<TopicsController> logger)
        {
            _topicService = topicService;
            _logger = logger;
        }

        // GET: api/<TopicsController>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation($"Received GET request to retrieve all topics");
            return Ok(await _topicService.GetAllTopicsAsync());
        }

        // GET api/<TopicsController>/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Received GET request to retrieve topic by ID {id}");
            var topic = await _topicService.GetTopicByIdAsync(id);
            return topic == null ? NotFound() : Ok(topic);
        }

        // POST api/<TopicsController>
        [HttpPost]
        [Authorize]
        public async Task<TopicResponseDTO> CreateTopic(CreateTopicRequest request)
        {
            _logger.LogInformation($"Received POST request to create new topic");
            return await _topicService.CreateTopicAsync(request);
        }

        // PUT api/<TopicsController>/
        [HttpPut]
        [Authorize]
        public async Task<TopicResponseDTO> Update(UpdateTopicRequest request)
        {
            _logger.LogInformation($"Received PUT request to update topic by ID {request.Id}");
            return await _topicService.UpdateTopicAsync(request);
        }

        // DELETE api/<TopicsController>/
        [HttpDelete]
        [Authorize]
        public async Task Delete(DeleteTopicRequest request)
        {
            _logger.LogInformation($"Received DELETE request to delete topic by ID {request.Id}");
            await _topicService.DeleteTopicAsync(request);
        }
    }
}
