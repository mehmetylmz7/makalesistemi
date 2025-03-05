using System;
using System.Collections.Generic;

namespace makalesistemi.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public List<Article>? Articles { get; set; }
    }
}
