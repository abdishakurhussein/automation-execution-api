using System.ComponentModel.DataAnnotations;

namespace AutomationExecution.API.Dtos
{
    public class CreateTestExecutionRequest
    {
        [Required]
        public string TestSuite { get; set; } = string.Empty;

        [Required]
        public string Environment { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}