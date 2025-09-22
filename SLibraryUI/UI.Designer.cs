namespace SLibraryUI
{
    partial class UI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Reservebutton = new System.Windows.Forms.Button();
            this.addBook = new System.Windows.Forms.Button();
            this.BookDGV = new System.Windows.Forms.DataGridView();
            this.Reservation = new System.Windows.Forms.TabPage();
            this.ReserveDGV = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.book_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bookTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReleaseDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BookID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Author = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Available = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reserved = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.release = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BookDGV)).BeginInit();
            this.Reservation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReserveDGV)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.Reservation);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 433);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.Reservebutton);
            this.tabPage1.Controls.Add(this.addBook);
            this.tabPage1.Controls.Add(this.BookDGV);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 407);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Books";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Reservebutton
            // 
            this.Reservebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Reservebutton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Reservebutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Reservebutton.Location = new System.Drawing.Point(673, 52);
            this.Reservebutton.Name = "Reservebutton";
            this.Reservebutton.Size = new System.Drawing.Size(111, 31);
            this.Reservebutton.TabIndex = 2;
            this.Reservebutton.Text = "Reserve";
            this.Reservebutton.UseVisualStyleBackColor = true;
            this.Reservebutton.Click += new System.EventHandler(this.Reservebutton_Click);
            // 
            // addBook
            // 
            this.addBook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addBook.Cursor = System.Windows.Forms.Cursors.Hand;
            this.addBook.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.addBook.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addBook.Location = new System.Drawing.Point(673, 17);
            this.addBook.Name = "addBook";
            this.addBook.Size = new System.Drawing.Size(111, 29);
            this.addBook.TabIndex = 1;
            this.addBook.Text = "Add Book";
            this.addBook.UseVisualStyleBackColor = true;
            this.addBook.Click += new System.EventHandler(this.addBook_Click);
            // 
            // BookDGV
            // 
            this.BookDGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.BookDGV.BackgroundColor = System.Drawing.SystemColors.MenuBar;
            this.BookDGV.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.BookDGV.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.BookDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BookDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BookID,
            this.Title,
            this.Author,
            this.Available,
            this.Reserved});
            this.BookDGV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BookDGV.GridColor = System.Drawing.Color.LightGray;
            this.BookDGV.Location = new System.Drawing.Point(3, 103);
            this.BookDGV.Name = "BookDGV";
            this.BookDGV.Size = new System.Drawing.Size(786, 301);
            this.BookDGV.TabIndex = 0;
            // 
            // Reservation
            // 
            this.Reservation.Controls.Add(this.release);
            this.Reservation.Controls.Add(this.ReserveDGV);
            this.Reservation.Location = new System.Drawing.Point(4, 22);
            this.Reservation.Name = "Reservation";
            this.Reservation.Padding = new System.Windows.Forms.Padding(3);
            this.Reservation.Size = new System.Drawing.Size(792, 407);
            this.Reservation.TabIndex = 1;
            this.Reservation.Text = "Reservation";
            this.Reservation.UseVisualStyleBackColor = true;
            // 
            // ReserveDGV
            // 
            this.ReserveDGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ReserveDGV.BackgroundColor = System.Drawing.SystemColors.MenuBar;
            this.ReserveDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ReserveDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.ClientName,
            this.book_ID,
            this.bookTitle,
            this.ResDate,
            this.ReleaseDate});
            this.ReserveDGV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ReserveDGV.GridColor = System.Drawing.SystemColors.MenuBar;
            this.ReserveDGV.Location = new System.Drawing.Point(3, 108);
            this.ReserveDGV.Name = "ReserveDGV";
            this.ReserveDGV.Size = new System.Drawing.Size(786, 296);
            this.ReserveDGV.TabIndex = 0;
            this.ReserveDGV.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ReserveDGV_CellContentClick);
            // 
            // ID
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ID.DefaultCellStyle = dataGridViewCellStyle6;
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            // 
            // ClientName
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ClientName.DefaultCellStyle = dataGridViewCellStyle7;
            this.ClientName.HeaderText = "Client Name";
            this.ClientName.Name = "ClientName";
            // 
            // book_ID
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.book_ID.DefaultCellStyle = dataGridViewCellStyle8;
            this.book_ID.HeaderText = "Book ID";
            this.book_ID.Name = "book_ID";
            // 
            // bookTitle
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.bookTitle.DefaultCellStyle = dataGridViewCellStyle9;
            this.bookTitle.HeaderText = "Book Title";
            this.bookTitle.Name = "bookTitle";
            // 
            // ResDate
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ResDate.DefaultCellStyle = dataGridViewCellStyle10;
            this.ResDate.HeaderText = "Reserve Date";
            this.ResDate.Name = "ResDate";
            // 
            // ReleaseDate
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ReleaseDate.DefaultCellStyle = dataGridViewCellStyle11;
            this.ReleaseDate.HeaderText = "Release Date";
            this.ReleaseDate.Name = "ReleaseDate";
            // 
            // BookID
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BookID.DefaultCellStyle = dataGridViewCellStyle1;
            this.BookID.HeaderText = "ID";
            this.BookID.Name = "BookID";
            this.BookID.ReadOnly = true;
            // 
            // Title
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.DefaultCellStyle = dataGridViewCellStyle2;
            this.Title.HeaderText = "Title";
            this.Title.Name = "Title";
            this.Title.ReadOnly = true;
            // 
            // Author
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Author.DefaultCellStyle = dataGridViewCellStyle3;
            this.Author.HeaderText = "Author";
            this.Author.Name = "Author";
            this.Author.ReadOnly = true;
            // 
            // Available
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Available.DefaultCellStyle = dataGridViewCellStyle4;
            this.Available.HeaderText = "Available";
            this.Available.Name = "Available";
            this.Available.ReadOnly = true;
            // 
            // Reserved
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Reserved.DefaultCellStyle = dataGridViewCellStyle5;
            this.Reserved.HeaderText = "Reserved";
            this.Reserved.Name = "Reserved";
            this.Reserved.ReadOnly = true;
            // 
            // release
            // 
            this.release.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.release.Cursor = System.Windows.Forms.Cursors.Hand;
            this.release.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.release.Location = new System.Drawing.Point(671, 38);
            this.release.Name = "release";
            this.release.Size = new System.Drawing.Size(101, 30);
            this.release.TabIndex = 2;
            this.release.Text = "Release";
            this.release.UseVisualStyleBackColor = true;
            this.release.Click += new System.EventHandler(this.Release_Click);
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "UI";
            this.Text = "SLibrary UI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BookDGV)).EndInit();
            this.Reservation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ReserveDGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage Reservation;
        private System.Windows.Forms.DataGridView BookDGV;
        private System.Windows.Forms.Button addBook;
        private System.Windows.Forms.DataGridView ReserveDGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn book_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn bookTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReleaseDate;
        private System.Windows.Forms.Button Reservebutton;
        private System.Windows.Forms.DataGridViewTextBoxColumn BookID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title;
        private System.Windows.Forms.DataGridViewTextBoxColumn Author;
        private System.Windows.Forms.DataGridViewTextBoxColumn Available;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reserved;
        private System.Windows.Forms.Button release;
    }
}

