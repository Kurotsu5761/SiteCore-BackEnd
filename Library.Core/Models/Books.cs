using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Models
{
    public class Books
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }
        
        public string ImageUrl { get; set; }

        public string BookStatus { get; set; }

        public User CurrentUser { get; set; }

        public DateTime DateRented { get; set; }

        public Category Category { get; set; }

        public List<Author> Authors { get; set; }
    }
}
