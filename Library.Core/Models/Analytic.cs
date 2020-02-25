using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Models
{
    public class Analytic
    {
        public string BookTitle { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime? DateRented { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; }
    }
}
