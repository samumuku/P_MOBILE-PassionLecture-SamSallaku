using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_PassionLecture.Models
{
    public class BookTag
    {
        public int BookId { get; set; }
        public int TagId { get; set; }

        public Tag Tag { get; set; }
    }
}
