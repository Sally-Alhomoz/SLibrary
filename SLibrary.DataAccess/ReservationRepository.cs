using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SLibrary.DataAccess
{
    public interface IReservationRepository
    {
        void Save();

        void Load();
        void Add(Reservation r);
        List<Reservation> GetReservation();
    }
    public class ReservationRepository :IReservationRepository
    {
        static string filePath = "ReservationsData.csv";
        private List<Reservation> reservations = new List<Reservation>();

        public ReservationRepository()
        {
            Load();
        }
        public void Save()
        {

            List<string> lines = new List<string>();
            lines.Add($"ID,Client_Name,Book_ID,Book_Title,Reserved_Date,Released_Date");

            foreach (Reservation r in reservations)
            {
                string released = r.ReleaseDate.HasValue ? r.ReleaseDate.Value.ToString() : "-";
                lines.Add($"{r.ClientID},{r.ClientName},{r.BookID},{r.BookTitle},{r.ReservedDate},{released}");
            }
            File.WriteAllLines(filePath, lines);
        }

        public void Load()
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines.Skip(1))
                {
                    var parts = line.Split(',');
                    if (parts.Length < 5)
                        continue;
                    DateTime? releaseDate = parts[5] == "-" ? (DateTime?)null : DateTime.Parse(parts[5]);
                    Reservation r = new Reservation(int.Parse(parts[0]), parts[1], int.Parse(parts[2]), parts[3], DateTime.Parse(parts[4]));
                    r.ReleaseDate = releaseDate;
                    reservations.Add(r);
                }
            }
        }

        public void Add(Reservation r)
        {
            if(reservations.Count>0)
            {
                var maxID = reservations.Max(i => i.ClientID);
                r.ClientID = maxID + 1;
            }
            else
            {
                r.ClientID = 1;
            }
            reservations.Add(r);
            Save();
        }

        public List<Reservation> GetReservation()
        {
            return reservations;
        }
    }
}
