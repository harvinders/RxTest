using System;
using System.Collections.Generic;
using System.Text;

namespace RxTest
{
    public class Article: IContent
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SourceUrl { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime LastActiveDate { get; set; }
        public string ImageUrl { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Url { get; set; }
    }
}
