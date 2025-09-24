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

        public FileReservationRepository(string filepath)
        {
            filePath = filepath;
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
            List<Reservation> list = Load();

            if (list.Count > 0)
            {
                var maxID = list.Max(i => i.ClientID);
                r.ClientID = maxID + 1;
            }
            else
            {
                r.ClientID = 1;
            }
            list.Add(r);
            Save(list);
        }
        public List<Reservation> GetReservation()
        {
            return Load();
        }

        public Reservation GetActiveReservation(string title, string clientName)
        {
            List<Reservation> list = Load();
            foreach(Reservation r in list)
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
            List<Reservation> list = Load();
            foreach (Reservation res in list)
            {
                if (res.ClientID == r.ClientID)
                {
                    res.BookTitle = r.BookTitle;
                    res.ClientName = r.ClientName;
                    res.BookID = r.BookID;
                    res.ReservedDate = r.ReservedDate;
                    res.ReleaseDate = DateTime.Now;

                    Save(list);
                    break;
                }
            }
        }
    }
}
