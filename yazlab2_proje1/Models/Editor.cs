using System;

namespace makalesistemi.Models
{
    public class Editor
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public List<Article>? ManagedArticles { get; set; }
    }
}
