using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLibraryUI
{
    public partial class Actions : Form
    {
        public string bookTitle => textBox1.Text.Trim();
        public string clientName => textBox2.Text.Trim();
        public string Mode { get; set; }
        public Actions(string mode)
        {
            InitializeComponent();
            Mode = mode;
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
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter both Title and Name.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
