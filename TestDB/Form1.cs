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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadBooksData();
        }
        private void LoadBooksData()
        {
            try
            {
                // 1. Получите контекст базы данных.
                using (var db = LibraryDBEntities.GetContext())
                {
                    // 2. Получите данные из таблицы "Книги".
                    var books = db.Books
                     .Select(b => new
                     {
                         b.BookID,
                         b.Title,
                         AuthorName = b.Authors.FullName, // обращаемся к полю FullName из таблицы Authors
                         b.PublicationYear,
                         b.Price
                     })
                     .ToList();
                    // 3. Привяжите данные к DataGridView.
                    dataGridViewBooks.DataSource = books;
                    // 4. Настройка DataGridView
                    FormatDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
        private void FormatDataGridView()
        {
            // Задаем заголовки столбцов
            dataGridViewBooks.Columns["BookID"].HeaderText = "ID Книги";
            dataGridViewBooks.Columns["Title"].HeaderText = "Название";
            dataGridViewBooks.Columns["AuthorName"].HeaderText = "Автор";
            dataGridViewBooks.Columns["PublicationYear"].HeaderText = "Год издания";
            dataGridViewBooks.Columns["Price"].HeaderText = "Цена";
            // Скрываем столбец AuthorID, если он больше не нужен
            if (dataGridViewBooks.Columns.Contains("AuthorID"))
            {
                dataGridViewBooks.Columns["AuthorID"].Visible = false;
            }
            // Форматирование столбца Price для отображения как валюты
            dataGridViewBooks.Columns["Price"].DefaultCellStyle.Format = "c";
            // Обработка Nullable<int> для PublicationYear
            dataGridViewBooks.Columns["PublicationYear"].DefaultCellStyle.NullValue = "";
            // Запрет редактирования данных
            dataGridViewBooks.ReadOnly = true;
            // Автоматическая подгонка ширины столбцов по содержимому
            dataGridViewBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }
    }
}
