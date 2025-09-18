using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Shared;
using SLibrary.Business;

namespace SLibraryConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BookManager bookMng = new BookManager();
            int id = 1;

            while (true)
            {
                Console.WriteLine("Menu");
                Console.WriteLine("1- Add Book");
                Console.WriteLine("2- Reserve Book");
                Console.WriteLine("3- Release Book");
                Console.WriteLine("4- List All Books");
                Console.WriteLine("5- Exit");
                Console.WriteLine("Enter your Choice : ");
                int choice = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
                switch (choice)
                {
                    case 1:
                        {
                            Console.WriteLine("Enter a Book Title : ");
                            string t = Console.ReadLine();
                            Book b = new Book(t, id, 1, 0);
                            bookMng.Add(b);
                        }
                        break;

                    case 2:
                        {

                            Console.WriteLine("Enter a Book Title to Reseve : ");
                            string title = Console.ReadLine();
                            string msg =bookMng.ReserveBook(title);
                            Console.WriteLine(msg);
                        }
                        break;

                    case 3:
                        {
                            Console.WriteLine("Enter a Book Title to Release : ");
                            string title = Console.ReadLine();
                            string msg = bookMng.ReleaseBook(title);
                            Console.WriteLine(msg);
                        }
                        break;

                    case 4:
                        {

                            Console.WriteLine(bookMng.ToString());
                            Console.WriteLine();
                        }
                        break;

                    case 5:
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
