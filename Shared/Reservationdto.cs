using System;

namespace Shared
{
    public class Reservationdto
    { 
        public int ID { set; get; }
        public string BookTitle { set; get; }
        public int BookID { set; get; }
        public string ClientName { set; get; }
        public string ReservedBy { set; get; }
        public DateTime ReservedDate { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public TimeSpan ReservationPeriod
        {
            get
            {
                if (ReleaseDate.HasValue)
                {
                    return ReleaseDate.Value.Subtract(ReservedDate);
                }
                return TimeSpan.Zero;
            }
        }

    }
}
