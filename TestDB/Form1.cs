using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestDB.Models;

namespace TestDB
{
    public partial class Form1 : Form
    {
        private List<BookDisplayItem> displayBooks = new List<BookDisplayItem>();
        private List<Books> originalBooks;

        // изменения
        List<BookDisplayItem> newBooks = new List<BookDisplayItem>();
        List<BookDisplayItem> changedBooks = new List<BookDisplayItem>();
        List<BookDisplayItem> deletedBooks = new List<BookDisplayItem>();

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadBooksData();
            InitializeComboBoxes();
            InitializeContextMenu();
        }
        private void FilterControls_Changed(object sender, EventArgs e)
        {
            ApplyFilters(); // Вызов метода для применения фильтров и сортировки
        }
        private void InitializeComboBoxes()
        {
            cmbFilter.Items.AddRange(new[] { "Все", "1980-2000", "До 2020", "После 2023" });
            cmbSort.Items.AddRange(new[] { "По названию", "По автору", "По году", "По цене" });
            cmbFilter.SelectedIndex = cmbSort.SelectedIndex = 0;
            // Подписка на события
            txtSearch.TextChanged += FilterControls_Changed;
            cmbFilter.SelectedIndexChanged += FilterControls_Changed;
            cmbSort.SelectedIndexChanged += FilterControls_Changed;
        }
        private void InitializeContextMenu()
        {
            ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("Удалить");
            ToolStripMenuItem addMenuItem = new ToolStripMenuItem("Добавить");

            ContextMenu.Items.AddRange(new[] { deleteMenuItem, addMenuItem });
            dataGridViewBooks.ContextMenuStrip = ContextMenu;

            deleteMenuItem.Click += DeleteItem_Click;
            addMenuItem.Click += AddItem_Click;
        }
        // удаление элемента
        private void DeleteItem_Click(object sender, EventArgs e)
        {
            SaveButton.Visible = true;
            CancelButton.Visible = true;
            DataGridViewSelectedRowCollection Rows = dataGridViewBooks.SelectedRows;
            if (Rows.Count > 0)
            {
                var result = MessageBox.Show($"Удалить {Rows.Count} записей?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                foreach (DataGridViewRow row in Rows)
                {
                    BookDisplayItem bookDisplay = displayBooks[row.Index];
                    if (!newBooks.Contains(bookDisplay))
                        deletedBooks.Add(bookDisplay);

                    displayBooks.Remove(bookDisplay);
                    newBooks.Remove(bookDisplay);
                    changedBooks.Remove(bookDisplay);
                }
            } 
            else
                MessageBox.Show("Выберите строку", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        // добавление элемента
        private void AddItem_Click(object sender, EventArgs e)
        {
            SaveButton.Visible = true;
            CancelButton.Visible = true;
            BookDisplayItem newBook = new BookDisplayItem(){
                BookID = -1,
                Title = "",
                PublicationYear = -1,
                Price = -1,
                AuthorName = ""};
            newBooks.Add(newBook);
            displayBooks.Add(newBook);
            dataGridViewBooks.DataSource = displayBooks;
            FormatDataGridView();
        }
        // изменение элемента
        private void dataGridViewBooks_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SaveButton.Visible = true;
            CancelButton.Visible = true;
            // Проверяем, что индекс строки корректен
            if (e.RowIndex >= 0)
            {
                int bookDisplayIndex = e.RowIndex;

                BookDisplayItem bookItem = displayBooks[bookDisplayIndex];

                if (!newBooks.Contains(bookItem) && !changedBooks.Contains(bookItem))
                   changedBooks.Add(bookItem);

                var value = dataGridViewBooks.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                switch (e.ColumnIndex)
                {
                    case 0:
                        bookItem.BookID = (int)value;
                        break;
                    case 1:
                        bookItem.Title = (string)value;
                        break;
                    case 2:
                        bookItem.AuthorName = (string)value;
                        break;
                    case 3:
                        bookItem.PublicationYear = (int)value;
                        break;
                    case 4:
                        bookItem.Price = (decimal)value;
                        break;
                }
            }
        }
        private void ApplyFilters()
        {
            // Получаем текст поиска, выбранный фильтр и сортировку
            string searchText = txtSearch.Text.Trim();
            string selectedFilter = cmbFilter.SelectedItem?.ToString();
            string selectedSort = cmbSort.SelectedItem?.ToString();
            // LINQ запрос для фильтрации
            var filteredBooks = displayBooks.AsQueryable();
            // Применяем поиск
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredBooks = filteredBooks.Where(b =>
                b.Title.ToLower().Contains(searchText.ToLower()) ||
                b.AuthorName.ToLower().Contains(searchText.ToLower()));
            }
            //Применение фильтра
            if (selectedFilter != "Все")
            {
                switch (selectedFilter)
                {
                    case "До 2020":
                        filteredBooks = filteredBooks.Where(b => b.PublicationYear < 2020);
                        break;
                    case "1980-2000":
                        filteredBooks = filteredBooks.Where(b => b.PublicationYear >= 1980 && b.PublicationYear <= 2000);
                        break;
                    case "После 2023":
                        filteredBooks = filteredBooks.Where(b => b.PublicationYear > 2023);
                        break;
                    default:
                        break;
                }
            }
            // Применяем сортировку
            switch (selectedSort)
            {
                case "По названию":
                    filteredBooks = filteredBooks.OrderBy(b => b.Title);
                    break;
                case "По автору":
                    filteredBooks = filteredBooks.OrderBy(b => b.AuthorName);
                    break;
                case "По году":
                    filteredBooks = filteredBooks.OrderBy(b => b.PublicationYear);
                    break;
                case "По цене":
                    filteredBooks = filteredBooks.OrderBy(b => b.Price);
                    break;
                default:
                    filteredBooks = filteredBooks.OrderBy(b => b.Title);
                    break;
            }
            // Обновляем DataGridView
            dataGridViewBooks.DataSource = filteredBooks.ToList();
        }
        private class BookDisplayItem
        {
            public string AuthorName { get; set; }
            public int BookID { get; set; }
            public string Title { get; set; }
            public int? PublicationYear { get; set; }
            public decimal? Price { get; set; }
        }

        private void LoadBooksData()
        {
            // 1. Получите контекст базы данных.
            using (var db = LibraryDBEntities.GetContext())
            {
                // 2. Получите данные из таблицы "Книги" и свяжите их с данными из таблицы "Авторы".
                //Загружаем данные с авторами
                originalBooks = db.Books
                //.Include(b => b.Authors)
                .ToList();
                // Конвертируем в отображаемый формат
                displayBooks = new List<BookDisplayItem>(originalBooks
                .Select(b => new BookDisplayItem
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    PublicationYear = b.PublicationYear,
                    Price = b.Price,
                    AuthorName = b.Authors?.FullName ?? "Неизвестный автор"
                })
                .ToList());
                // 3. Привяжите данные к DataGridView.
                dataGridViewBooks.DataSource = displayBooks;
                // 4. Настройка DataGridView
                FormatDataGridView();
            }
        }

        private void FormatDataGridView()
        {
            dataGridViewBooks.ReadOnly = false;
            dataGridViewBooks.AutoGenerateColumns = false;
            dataGridViewBooks.Columns.Clear();
            // Настраиваем столбцы
            dataGridViewBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BookID",
                HeaderText = "ID Книги"
            });
            dataGridViewBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Title",
                HeaderText = "Название"
            });
            dataGridViewBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AuthorName",
                HeaderText = "Автор"
            });
            dataGridViewBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PublicationYear",
                HeaderText = "Год издания"
            });
            dataGridViewBooks.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price",
                HeaderText = "Цена",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // TODO: Реализовать сохранение
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            // TODO: Реализовать отмену изменений
        }
    }
}
