using AutomationExecution.API.Dtos;
using AutomationExecution.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace TestExecutions.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExecutionsController : ControllerBase
    {
        // This is where the response logic goes into, in our case, this is the outcome it displays upon trying out and executing
        private static readonly List<TestExecution> Executions =
[
                new TestExecution
                {
                    Id = 1,
                    TestSuite = "Checkout Regression",
                    Environment = "QA",
                    Status = "Passed",
                    StartedAt = DateTime.UtcNow.AddMinutes(-5),
                    CompletedAt = DateTime.UtcNow
                },

                new TestExecution
                {
                    Id = 2,
                    TestSuite = "Login Smoke Tests",
                    Environment = "TEST",
                    Status = "Running",
                    StartedAt = DateTime.UtcNow,
                    CompletedAt = null
                },

                new TestExecution
                {
                    Id = 3,
                    TestSuite = "Custom Abdi Test",
                    Environment = "QA",
                    Status = "Passed",
                    StartedAt = DateTime.UtcNow.AddHours(-2),
                    CompletedAt = null
                }
            ];
        // This is the Restful method, in this case, the API is a GET, and our very first endpoint!
        [HttpGet]
        public ActionResult<IEnumerable<TestExecution>> GetAll()
        {
            return Ok(Executions);
        }

        // Here is the second endpoint -- GET by ID
        [HttpGet("{id}")]
        public ActionResult<TestExecution> GetById(int id) 
        {
            var execution = Executions.FirstOrDefault(x => x.Id == id);

            if (execution == null) 
            {
                return NotFound();
            }
            // In this case, return behaves like "otherwise", rather than else, right now it'll only recognise id's 1, 2 and 3 (3 is custom made by me)
            return Ok(execution);
        }

        // Third endpoint - PUT
        [HttpPut("{id}")]
        public ActionResult<TestExecution> PutById(
        int id,
        [FromBody] UpdateTestExecutionRequest request)
            {
                var execution = Executions.FirstOrDefault(x => x.Id == id);

                if (execution == null)
                {
                    return NotFound();
                }

                execution.TestSuite = request.TestSuite;
                execution.Environment = request.Environment;
                execution.Status = request.Status;
                execution.StartedAt = request.StartedAt;
                execution.CompletedAt = request.CompletedAt;

                return Ok(execution);
            }

        // Fourth endpoint - POST
        [HttpPost]
        public ActionResult<TestExecution> CreateExecution(
        [FromBody] CreateTestExecutionRequest request)
            {
                var nextId = Executions.Any()
                    ? Executions.Max(x => x.Id) + 1
                    : 1;

                var newExecution = new TestExecution
                {
                    Id = nextId,
                    TestSuite = request.TestSuite,
                    Environment = request.Environment,
                    Status = request.Status,
                    StartedAt = request.StartedAt,
                    CompletedAt = request.CompletedAt
                };

                Executions.Add(newExecution);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = newExecution.Id },
                    newExecution);
        }

        // Fifth endpoint - DELETE
        [HttpDelete("{id}")]
        public ActionResult DeleteExecution(int id)
        {
            var execution = Executions.FirstOrDefault(x => x.Id == id);

            if (execution == null)
            {
                return NotFound();
            }

            Executions.Remove(execution);

            return NoContent();
        }
    }
    
}
