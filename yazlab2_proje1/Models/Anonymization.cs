namespace makalesistemi.Models
{
    public class Anonymization
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string OriginalText { get; set; } = string.Empty;
        public string AnonymizedText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Article? Article { get; set; }
    }
}
