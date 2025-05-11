using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassRoomClone_App.Server.Controllers
{
    [Route("api/classes/{classId:int}/")]
    public class ClassWorkController : ControllerBase
    {
        private readonly IClassWorkService _classWorkService;

        public ClassWorkController(IClassWorkService classWorkService)
        {
            _classWorkService = classWorkService;
        }
        
        [HttpGet("class-works")]
        public async Task<ActionResult<List<TopicWithMaterialsAssignmentsDto>>> GetTopicsWithMaterialsAndAssignments(int classId)
        {
            var result = await _classWorkService.GetTopicsWithMaterialsAndAssignmentsAsync(classId);
            return Ok(result);
        }
    }
}
