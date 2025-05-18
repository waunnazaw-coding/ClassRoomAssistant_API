using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Helpers;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClassRoomClone_App.Server.Controllers
{
    [Route("api/classes/{classId:int}")]
    [ApiController]
    public class ClassWorkController : ControllerBase
    {
        private readonly IClassWorkService _classWorkService;

        public ClassWorkController(IClassWorkService classWorkService)
        {
            _classWorkService = classWorkService;
        }

        /// <summary>
        /// Get all topics for a class along with their materials and assignments.
        /// </summary>
        [HttpGet("topics-with-details")]
        public async Task<ActionResult<ApiResponse<List<TopicWithMaterialsAssignmentsDto>>>> GetTopicsWithMaterialsAndAssignments(int classId)
        {
            var result = await _classWorkService.GetTopicsWithMaterialsAndAssignmentsAsync(classId);

            var response = new ApiResponse<List<TopicWithMaterialsAssignmentsDto>>(result , true , "Topic Retrieve Successfully");

            return Ok(response);
        }

        /// <summary>
        /// Get detailed information for a specific topic within a class.
        /// </summary>
        [HttpGet("topics/{topicId:int}")]
        public async Task<ActionResult<ApiResponse<TopicWithMaterialsAssignmentsDto>>> FilterByTopic(int classId, int topicId)
        {
            var result = await _classWorkService.FilterByTopicAsync(topicId, classId);

            if (result == null)
            {
                return NotFound(new ApiResponse<TopicWithMaterialsAssignmentsDto>(null, false, "Topic not found"));
            }

            var response = new ApiResponse<TopicWithMaterialsAssignmentsDto>(result , true , "Topic Retrieve Successfully");

            return Ok(response);
        }
    }
}