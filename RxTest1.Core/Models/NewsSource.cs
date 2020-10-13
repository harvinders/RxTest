using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RxTest
{
    public class NewsSource: IContent
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SourceUrl { get; set; }
        public DateTime PublicationDate { get; set; }
        public string ImageUrl { get; set; }
        public string Author { get; set; } = string.Empty;
        public string HomeUrl { get; set; }
        // Relationships
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
