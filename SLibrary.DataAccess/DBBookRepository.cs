using Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLibrary.DataAccess
{
    public class DBBookRepository : IBookRepository
    {
        //"Server=.\\SQLEXPRESS;Database=SLibrary;Trusted_Connection=True";
        private string connectionString;
        public DBBookRepository(string connString)
        {
            connectionString = connString;
        }

        public List<Book> Load()
        {
            List<Book> books = new List<Book>();

            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID, Title, Author, Available, Reserved FROM Books";
                using(SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            Book book = new Book()
                            {
                                ID = (int)reader["ID"],
                                Title = (string)reader["Title"],
                                Author = (string)reader["Author"],
                                AvailableCount = (int)reader["Available"],
                                ReservedCount = (int)reader["Reserved"]
                            };
                            books.Add(book);
                        }
                    }
                }
            }
            return books;
        }
        public void Add(Book b)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string check = "SELECT ID, Available FROM Books WHERE Title = @title";
                using (SqlCommand checkcmd = new SqlCommand(check, conn))
                {
                    checkcmd.Parameters.AddWithValue("@title", b.Title);

                    using (SqlDataReader reader = checkcmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = (int)reader["ID"];
                            int available = (int)reader["Available"];

                            reader.Close();

                            string update = "UPDATE Books SET Available = @available WHERE ID = @id";

                            using (SqlCommand updatecmd = new SqlCommand(update, conn))
                            {
                                updatecmd.Parameters.AddWithValue("@available", available + 1);
                                updatecmd.Parameters.AddWithValue("@id", id);
                                updatecmd.ExecuteNonQuery();
                            }
                            return;
                        }
                    }
                }

                string insert = "INSERT INTO Books (Title, Author, Available, Reserved) VALUES (@title, @author, @available, @res)";
                using (SqlCommand insertCmd = new SqlCommand(insert, conn))
                {
                    insertCmd.Parameters.AddWithValue("@title", b.Title);
                    insertCmd.Parameters.AddWithValue("@author", b.Author);
                    insertCmd.Parameters.AddWithValue("@available", b.AvailableCount);
                    insertCmd.Parameters.AddWithValue("@res", b.ReservedCount);

                    insertCmd.ExecuteNonQuery();
                }
            }

        }

        public Book GetByName(string title)
        {
            Book b = new Book();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT ID, Title, Author, Available, Reserved FROM Books WHERE Title = @title";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book()
                            {
                                ID = (int)reader["ID"],
                                Title = (string)reader["Title"],
                                Author = (string)reader["Author"],
                                AvailableCount = (int)reader["Available"],
                                ReservedCount = (int)reader["Reserved"]
                            };
                            b = book;
                        }
                    }
                }
            }

            return b;
        }

        public int BookCount()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Books";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public List<Book> GetList()
        {
            List<Book> books = new List<Book>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID, Title, Author, Available, Reserved FROM Books";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Book book = new Book()
                        {
                            ID = (int)reader["ID"],
                            Title = (string)reader["Title"],
                            Author = (string)reader["Author"],
                            AvailableCount = (int)reader["Available"],
                            ReservedCount = (int)reader["Reserved"]
                        };
                        books.Add(book);
                    }
                }
            }

            return books;
        }


        public void UpdateCounts(int bookId, int available, int reserved)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Books SET Available=@avail, Reserved=@res WHERE ID=@id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@avail", available);
                    cmd.Parameters.AddWithValue("@res", reserved);
                    cmd.Parameters.AddWithValue("@id", bookId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
