using System;
using System.ComponentModel.DataAnnotations;

namespace SLibrary.DataAccess.Models
{
    public class Reservation
    {
        [Key]
        public int ID { set; get; }
        public string BookTitle { set; get; }
        public int BookID { set; get; }
        public string ClientName { set; get; }
        public string ReservedBy { set; get; }
        public DateTime ReservedDate { get; set; }
        public DateTime? ReleaseDate { get; set; }

    }
}
