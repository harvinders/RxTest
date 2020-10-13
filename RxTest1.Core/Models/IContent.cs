using System;
using System.Collections.Generic;
using System.Text;

namespace RxTest
{
    public interface IContent
    {
        string Title { get; set; }
        string Description { get; set; } 
        string SourceUrl { get; set; }
        DateTime PublicationDate { get; set; }
        string ImageUrl { get; set; }
        string Author { get; set; }
    }
}
