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
        public string Author { get; set; }
        public int ID { get; set; }
        public int bookCount { get; set; } = 0;
        public int ReservedCount { get; set; }
        public int AvailableCount { get; set; }


        public Book()
        {
            Title = "None";
            ID = -1;
            Author = "None";
            bookCount = 1;
            AvailableCount = 1;
            ReservedCount = 0;
        }

        public Book(int id , string title, string author,int availableCount, int reservedCount)
        {
            Title = title;
            ID = id;
            Author = author;
            AvailableCount = availableCount;
            ReservedCount = reservedCount;
            bookCount = ReservedCount + AvailableCount;
        }
    }

    public class Reservation
    {
        public string BookTitle { set; get; }
        public int ClientID { set; get; }
        public int BookID { set; get; }
        public string ClientName { set; get; }
        public DateTime ReservedDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public Reservation()
        {
            BookTitle = "None";
            ClientID = -1;
            BookID = -1;
            ClientName = "None";
            ReservedDate = DateTime.MinValue;
            ReleaseDate = null;
        }
        public Reservation(int clientId , string clientName , int bookid, string bookTitle, DateTime date)
        {
            BookTitle = bookTitle;
            ClientID = clientId;
            ClientName = clientName;
            ReservedDate = date;
            BookID = bookid;
        }
    }
}
