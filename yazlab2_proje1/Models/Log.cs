namespace makalesistemi.Models
{
    public class Log
    {
        public int Id { get; set; }
        public int? ArticleId { get; set; }
        public int PerformedBy { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Author? Author { get; set; }
        public Editor? Editor { get; set; }
        public Reviewer? Reviewer { get; set; }
        public Article? Article { get; set; }
    }
}
