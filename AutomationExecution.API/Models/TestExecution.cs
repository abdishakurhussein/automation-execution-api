namespace AutomationExecution.API.Models
{
    // Class
    public class TestExecution
    {
        // Data Type (int, string, DateTime) and Properties (Id, TestSuite, Environment, Status, StartedAt, CompletedAt)
        // Public means that the property can be accessed from outside the class.
        // Get means that the property can be read.
        // Set means that the property can be modified.
        public int Id { get; set; }

        public string TestSuite { get; set; } = string.Empty;
        
        public string Environment { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}