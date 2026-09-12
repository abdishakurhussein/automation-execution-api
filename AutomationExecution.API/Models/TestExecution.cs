namespace AutomationExecution.API.Models
{
    public class TestExecution
    {
        public int Id { get; set; }

        public string TestSuite { get; set; } = string.Empty;
        
        public string Environment { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}