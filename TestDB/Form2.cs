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
            // TODO: добавить события для кнопок с возвратом значений
        }
        private void InitializeComboBoxes()
        {
            var authors = LibraryDBEntities.GetContext().Authors;

            cmb.DataSource = authors.ToList(); // привязываем данные к ComboBox
            cmb.DisplayMember = "FullName"; // привязываем ячейку FullName к отображению 
            cmb.ValueMember = "AuthorID"; // привязываем AuthorID как значение элемента
        }
        }


    }
}
