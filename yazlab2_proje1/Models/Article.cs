using makalesistemi.Models;
using System;
using System.Collections.Generic;

namespace makalesistemi.Models
{
    public enum ArticleStatus
    {
        Beklemede,
        İnceleniyor,
        RevizeEdildi,
        Onaylandı,
        Reddedildi
    }
    public class Article
    {
        public int Id { get; set; }
        public int AuthorId { get; set; } // Yazar FK
        public int? ReviewerId { get; set; } // Hakem FK
        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public ArticleStatus Status { get; set; } = ArticleStatus.Beklemede;
        public Guid TrackingNumber { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Author? Author { get; set; }
        public Reviewer? Reviewer { get; set; }
    }
}
