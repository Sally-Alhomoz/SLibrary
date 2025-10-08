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
using SLibrary.Business.Managers;


namespace SLibraryUI
{
    public partial class UI : Form
    {
        BookManager bookMng = new BookManager();
        public UI()
        {
            InitializeComponent();
            SetupDataGrid();
            FillBookGrid();
            FillReserveGrid();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SetupDataGrid()
        {
            BookDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            BookDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            ReserveDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ReserveDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

        }



        private void FillBookGrid()
        {
            BookDGV.Rows.Clear();
            foreach(Book b in bookMng.GetAllBooks())
            {
                BookDGV.Rows.Add(b.ID, b.Title,b.Author, b.Available,b.Reserved);
            }
        }

        private void FillReserveGrid()
        {
            ReserveDGV.Rows.Clear();
            foreach (Reservation r in bookMng.GetAllReservations())
            {
                ReserveDGV.Rows.Add(r.ID, r.ClientName, r.BookID, r.BookTitle, r.ReservedDate, r.ReleaseDate?.ToString() ?? "-");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void addBook_Click(object sender, EventArgs e)
        {
            using (AddBook form = new AddBook())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Book newBook = new Book(form.BookTitle, form.BookAuthor, 1, 0);
                    bookMng.Add(newBook);
                    FillBookGrid();
                }
                else
                { }
            }
        }

        private void Release_Click(object sender, EventArgs e)
        {
            if (ReserveDGV.SelectedRows.Count > 0)
            {
                DataGridViewRow row = ReserveDGV.SelectedRows[0];

                string title = row.Cells["bookTitle"].Value.ToString();
                string name = row.Cells["ClientName"].Value.ToString();

                using (Actions form = new Actions("Release", title, name))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        string result = bookMng.ReleaseBook(title,name);
                        MessageBox.Show(result, "Reservation Status", MessageBoxButtons.OK, result.Contains("Successfully")
                         ? MessageBoxIcon.Information
                        : MessageBoxIcon.Warning);
                        FillBookGrid();
                        FillReserveGrid();
                    }
                    else
                    { }
                }
            }
            else
            {
                MessageBox.Show("Please select a Book first.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void ReserveDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Reservebutton_Click(object sender, EventArgs e)
        {
            if (BookDGV.SelectedRows.Count > 0)
            {
                DataGridViewRow row = BookDGV.SelectedRows[0];

                string title = row.Cells["Title"].Value.ToString();

                using (Actions form = new Actions("Reserve", title))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        string result = bookMng.ReserveBook(title, form.clientName);
                        MessageBox.Show(result, "Reservation Status", MessageBoxButtons.OK, result.Contains("Successfully")
                         ? MessageBoxIcon.Information
                        : MessageBoxIcon.Warning);
                        FillBookGrid();
                        FillReserveGrid();
                    }
                    else
                    { }
                }
            }
            else
            {
                MessageBox.Show("Please select a Book first.","Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

    }
}
