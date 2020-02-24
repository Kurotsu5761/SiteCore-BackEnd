using Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteCore_BackEnd.Models
{
    public class PaginatedBook
    {
        public int Total { get; set; }
        public List<Books> Books { get; set; }

       public PaginatedBook(List<Books> books , int total)
        {
            Total = total;
            Books = books;
        }
    }
}
