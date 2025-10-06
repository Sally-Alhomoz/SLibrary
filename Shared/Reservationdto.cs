using System;

namespace Shared
{
    public class Reservationdto
    { 
        public int ID { set; get; }
        public string BookTitle { set; get; }
        public int BookID { set; get; }
        public string ClientName { set; get; }
        public DateTime ReservedDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}
