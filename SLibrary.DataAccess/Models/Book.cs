using System;
using System.ComponentModel.DataAnnotations;


namespace SLibrary.DataAccess.Models
{
    public class Book
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Reserved { get; set; }
        public int Available { get; set; }

        public bool isDeleted { get; set; } = false;
    }
}
