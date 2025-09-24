using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Shared;

namespace SLibrary.DataAccess
{
    public class RepositoryFactory
    {
        public static IBookRepository CreateBookRepository()
        {
            string mode = Config.Configuration["Storage:Mode"] ?? "Database"; 

            if(mode =="File")
            {
                string fp = Config.Configuration["Storage:BookFilePath"] ?? "books.json";
                return new FileBookRepository(fp);
            }

            string cs = Config.Configuration["Storage:ConnectionString"];
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("Missing Storage:ConnectionString in appsettings.json");

            return new DBBookRepository(cs);
        }

        public static IReservationRepository CreateReservationRepository()
        {
            string mode = Config.Configuration["Storage:Mode"] ?? "Database";

            if (mode == "File")
            {
                string fp = Config.Configuration["Storage:ReservationFilePath"] ?? "reservations.json";
                return new FileReservationRepository(fp);
            }

            string cs = Config.Configuration["Storage:ConnectionString"];
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("Missing Storage:ConnectionString in appsettings.json");

            return new DBReservationRepository(cs);
        }
    }
}
