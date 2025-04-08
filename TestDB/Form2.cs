using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestDB.Models;

namespace TestDB
{
    public partial class Form2 : Form
    {
        // айди выбранного автора
        public int authorId { get; private set; }
        public Form2()
        {
            InitializeComponent();
            InitializeComboBoxes();
            InitializeButtons();
        }
        private void InitializeComboBoxes()
        {
            var authors = LibraryDBEntities.GetContext().Authors;

            cmb.DataSource = authors.ToList(); // привязываем данные к ComboBox
            cmb.DisplayMember = "FullName"; // привязываем ячейку FullName к отображению 
            cmb.ValueMember = "AuthorID"; // привязываем AuthorID как значение элемента
        }
        private void InitializeButtons()
        {
            Cancel.Click += CancelButton_Click;
            Save.Click += SaveButton_Click;
        }
        private void CancelButton_Click (object sender, EventArgs e)
        {
            // возвращаем 0, в качестве отмены 
            authorId = 0;
            this.Close();
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // возвращаем айдишник 
            authorId = (int)cmb.SelectedValue;
            this.Close();
        }

    }
}
