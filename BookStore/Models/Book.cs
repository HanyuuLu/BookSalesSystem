using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Book
    {
        public string isbn;
        public string title;
        public string author;
        public int reserve;
        public int? quantity;
    }
}
