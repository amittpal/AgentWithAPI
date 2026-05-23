using AgentAPI.Agents;
using AgentAPI.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace AgentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly StudentAgent agent;
        public StudentController(StudentAgent studentAgent)
        {
            agent = studentAgent;
        }

        [HttpPost("StudentsAgent")]
        public async Task<ActionResult<Model.ChatResponse>> StudentsAgent([FromBody] ChatRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await agent.ChatAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (OperationCanceledException)
            {
                return BadRequest("Request was cancelled.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
