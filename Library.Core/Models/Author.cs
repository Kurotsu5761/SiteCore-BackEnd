using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Models
{
    public class Author
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public List<Books> Publishes { get; set; }
    }
}
