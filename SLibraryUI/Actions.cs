using SLibrary.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace SLibraryUI
{
    public partial class Actions : Form
    {
        private BookManager bookMng = new BookManager();
        public string clientName => clientNametxt.Text.Trim();
        public string Mode { get; set; }
        public Actions(string mode , string title, string clientName ="" )
        {
            InitializeComponent();
            Mode = mode;
            BookTitle.Text = title;

            if(Mode == "Release")
            {
                clientNametxt.Text = clientName;
                clientNametxt.ReadOnly = true;
            }

        }

        private void Actions_Load(object sender, EventArgs e)
        {
            if (Mode == "Reserve")
            {
                this.Text = "Reserve a Book";
                confirmButton.Text = "Reserve";
            }
            else
            {
                this.Text = "Release a Book";
                confirmButton.Text = "Release";
            }
        }

        private void Confirmbutton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(clientNametxt.Text))
            {
                MessageBox.Show("Please enter Name.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
