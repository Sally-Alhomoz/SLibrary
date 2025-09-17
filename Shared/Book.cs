using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Book
    {
        public string Title { get; set; }
        public int ID { get; set; }
        public int bookCount { get; set; } = 0;
        public int ReservedCount { get; set; }
        public int AvailableCount { get; set; }


        public Book()
        {
            Title = "None";
            ID = -1;
            bookCount = 1;
            AvailableCount = 1;
            ReservedCount = 0;
        }

        public Book(string title, int id, int availableCount, int reservedCount)
        {
            Title = title;
            ID = id;
            AvailableCount = availableCount;
            ReservedCount = reservedCount;
            bookCount = ReservedCount + AvailableCount;
        }
    }
}
