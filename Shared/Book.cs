using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class Book
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Reserved { get; set; }
        public int Available { get; set; }


        public Book()
        {
            Title = "None";
            Author = "None";
            Available = 1;
            Reserved = 0;
        }

        public Book(string title, string author,int availableCount, int reservedCount)
        {
            Title = title;
            Author = author;
            Available = availableCount;
            Reserved = reservedCount;
        }
    }
}
