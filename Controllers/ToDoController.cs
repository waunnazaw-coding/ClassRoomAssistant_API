using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassRoomClone_App.Server.Controllers
{
    [ApiController]
    [Route("api/todos")]
    public class ToDoController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public ToDoController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet("todo-tasks")]
        public async Task<ActionResult<IEnumerable<AssignmentWithStatusDto>>> GetAssignmentsWithStatus(
            [FromQuery] int userId)
        {
            var result = await _assignmentService.GetAssignmentsWithStatusAsync(userId);
            return Ok(result);
        }
    }
}
