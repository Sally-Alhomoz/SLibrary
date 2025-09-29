using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { set; get; }
        public string BookTitle { set; get; }
        public int BookID { set; get; }
        public string ClientName { set; get; }
        public DateTime ReservedDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public Reservation()
        {
            BookTitle = "None";
            BookID = -1;
            ClientName = "None";
            ReservedDate = DateTime.MinValue;
            ReleaseDate = null;
        }
        public Reservation(string clientName, int bookid, string bookTitle, DateTime date)
        {
            BookTitle = bookTitle;
            ClientName = clientName;
            ReservedDate = date;
            BookID = bookid;
        }
    }
}
