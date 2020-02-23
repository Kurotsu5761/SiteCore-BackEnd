using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteCore_BackEnd.Models
{
    public class BookModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string ImageUrl { get; set; }

        public List<int> AuthorIds { get; set; }

        public string BookStatus { get; set; }

        public List<AuthorModel> Author { get; set; }
    }
}
