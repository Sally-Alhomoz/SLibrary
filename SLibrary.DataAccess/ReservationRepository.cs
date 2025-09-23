using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace SLibrary.DataAccess
{
    public interface IReservationRepository
    {
        DataTable Load();
        void Add(Reservation r);
        List<Reservation> GetReservation();
        Reservation GetActiveReservation(string title, string clientName);
        void Update(Reservation r);
    }
    public class ReservationRepository :IReservationRepository
    {

        private string connectionString = "Server=.\\SQLEXPRESS;Database=SLibrary;Trusted_Connection=True";

        public ReservationRepository()
        {
           // Load();
        }

        public DataTable Load()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Reservations", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public void Add(Reservation r)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string insert = "INSERT INTO Reservations (Client_Name, Book_ID, Book_Title, ReservedDate, ReleaseDate) VALUES (@cname, @bid, @btitle, @rdate, @rel)";

                using (SqlCommand cmd = new SqlCommand(insert, conn))
                {
                    cmd.Parameters.AddWithValue("@cname", r.ClientName);
                    cmd.Parameters.AddWithValue("@bid", r.BookID);
                    cmd.Parameters.AddWithValue("@btitle", r.BookTitle);
                    cmd.Parameters.AddWithValue("@rdate", r.ReservedDate);
                    cmd.Parameters.AddWithValue("@rel", r.ReleaseDate.HasValue ? (object)r.ReleaseDate.Value : DBNull.Value);


                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Reservation> GetReservation()
        {
            List<Reservation> reservations = new List<Reservation>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Reservations";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Reservation res = new Reservation()
                        {
                            ClientID = (int)reader["ID"],
                            ClientName = (string)reader["Client_Name"],
                            BookID = (int)reader["Book_ID"],
                            BookTitle = (string)reader["Book_Title"],
                            ReservedDate = (DateTime)reader["ReservedDate"],
                            ReleaseDate = reader["ReleaseDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ReleaseDate"]
                        };
                        reservations.Add(res);
                    }
                }
            }
            return reservations;
        }

        public Reservation GetActiveReservation(string title, string clientName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT TOP 1 * FROM Reservations
                         WHERE Book_Title=@title AND Client_Name=@client AND ReleaseDate IS NULL
                         ORDER BY ReservedDate DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@client", clientName);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Reservation r = new Reservation()
                            {
                                ClientID = (int)reader["ID"],
                                ClientName = (string)reader["Client_Name"],
                                BookID = (int)reader["Book_ID"],
                                BookTitle = (string)reader["Book_Title"],
                                ReservedDate = (DateTime)reader["ReservedDate"],
                                ReleaseDate = reader["ReleaseDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ReleaseDate"]
                            };
                            return r;
                        }
                    }
                }
            }
            return null;
        }

        public void Update(Reservation r)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Reservations SET ReleaseDate=@rel WHERE ID=@id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@rel", r.ReleaseDate.Value);
                    cmd.Parameters.AddWithValue("@id", r.ClientID); 
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
