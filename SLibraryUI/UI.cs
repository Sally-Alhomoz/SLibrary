using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Shared;
using SLibrary.Business;


namespace SLibraryUI
{
    public partial class UI : Form
    {
        BookManager bookMng = new BookManager();
        public UI()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int choice = Convert.ToInt32(textBox1.Text);

            switch(choice)
            {
                case 1:
                    {
                        AddBook();
                    }break;

                case 2:
                    {
                        ReserveBook();
                    }
                    break;

                case 3:
                    {
                        ReleaseBook();
                    }
                    break;

                case 4:
                    {
                        MessageBox.Show(bookMng.ToString(), "List of Books");
                    }
                    break;
            }
           
        }

        private void AddBook()
        {
            string title = Interaction.InputBox("Enter the book title ", "Add Book");

            Book b = new Book(title, 0, 1, 0);
            bookMng.Add(b);
            MessageBox.Show($"Book {title} added !");
        }

        private void ReserveBook()
        {
            string title = Interaction.InputBox("Enter book title to Reserve ", "Reserve Book");

            string msg = bookMng.ReserveBook(title);
            MessageBox.Show(msg);
        }

        private void ReleaseBook()
        {
            string title = Interaction.InputBox("Enter book title to Release ", "Release Book");

            string msg=  bookMng.ReleaseBook(title);
            MessageBox.Show(msg);
        }
    }
}
