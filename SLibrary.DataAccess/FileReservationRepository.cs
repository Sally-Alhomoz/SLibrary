using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace SLibrary.DataAccess
{
    public class FileReservationRepository : IReservationRepository
    {
        private string filePath;
        private List<Reservation> reservations;
        public FileReservationRepository(string filepath)
        {
            filePath = filepath;
            reservations = Load();
        }
        public List<Reservation> Load()
        {
            if (!File.Exists(filePath))
                return new List<Reservation>();

            var json = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<Reservation>();
            return JsonSerializer.Deserialize<List<Reservation>>(json) ?? new List<Reservation>();
        }

        private void Save(List<Reservation> list)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            File.WriteAllText(filePath, JsonSerializer.Serialize(list, options));
        }
        public void Add(Reservation r)
        {
            if (reservations.Count > 0)
            {
                var maxID = reservations.Max(i => i.ID);
                r.ID = maxID + 1;
            }
            else
            {
                r.ID = 1;
            }
            reservations.Add(r);
            Save(reservations);
        }
        public List<Reservation> GetReservation()
        {
            return Load();
        }

        public Reservation GetActiveReservation(string title, string clientName)
        {
            foreach(Reservation r in reservations)
            {
                if (r.BookTitle == title && r.ClientName == clientName && r.ReleaseDate == null)
                {
                    return r;
                }
            }
            return null;
        }
        public void Update(Reservation r)
        {
            foreach (Reservation res in reservations)
            {
                if (res.ID == r.ID)
                {
                    res.BookTitle = r.BookTitle;
                    res.ClientName = r.ClientName;
                    res.BookID = r.BookID;
                    res.ReservedDate = r.ReservedDate;
                    res.ReleaseDate = DateTime.Now;

                    Save(reservations);
                    break;
                }
            }
        }
    }
}
