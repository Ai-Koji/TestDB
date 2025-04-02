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
        private List<BookDisplayItem> displayBooks = new List<BookDisplayItem>();
        private List<Books> originalBooks;
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

            deleteMenuItem.Click += DeleteMenuItem_Click;
            addMenuItem.Click += AddMenuItem_Click;
        }
        // удаление элемента
        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: реализовать удаление элементов
        }
        // добавление элемента
        private void AddMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: реализовать удаление элементов
        }
        // изменение элемента
        private void dataGridViewBooks_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // TODO: Реализовать событие изменения
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
