using BookSeek.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using BookSeek.Windows;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace BookSeek.Pages
{
    /// <summary>
    /// Interaction logic for Inventory.xaml
    /// </summary>
    public partial class Inventory : Page
    {
        Database database = new Database();
        List<BookInInventory> booksInInventory = new List<BookInInventory>();
        List<BookInInventory> deactivatedBooks = new List<BookInInventory>();
        public Inventory()
        {
            InitializeComponent();
            InitializeInventoryDataGrid();
            InitializeArchivesDataGrid();
        }

        private void InitializeInventoryDataGrid()
        {
            List<Book> books = BookOnDataGrid.GetActiveBooks(database);
            foreach (Book book in books)
            {
                BookInInventory bookInInventory = new BookInInventory();
                string path = Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\Images\Books\");
                string source = path + book.Isbn + ".jpg";
                bookInInventory.CoverImageSource = ImageUtility.GetBitmapImage(source);
                bookInInventory.Isbn = book.Isbn;
                bookInInventory.Title = book.Title;
                bookInInventory.Author = book.Author;
                bookInInventory.Price = book.Price;
                bookInInventory.Quantity = book.BookQuantityOnHand;
                bookInInventory.Publisher = book.Publisher;
                bookInInventory.DateOfPublication = book.DateOfPublication;
                booksInInventory.Add(bookInInventory);
            }
            InventoryDataGrid.ItemsSource = booksInInventory;
        }

        private void InitializeArchivesDataGrid()
        {
            List<Book> books = BookOnDataGrid.GetDeactivatedBooks(database);
            foreach (Book book in books)
            {
                BookInInventory bookInArchives = new BookInInventory();
                string path = Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\Images\Books\");
                string source = path + book.Isbn + ".jpg";
                bookInArchives.CoverImageSource = ImageUtility.GetBitmapImage(source);
                bookInArchives.Isbn = book.Isbn;
                bookInArchives.Title = book.Title;
                bookInArchives.Author = book.Author;
                bookInArchives.Price = book.Price;
                bookInArchives.Quantity = book.BookQuantityOnHand;
                bookInArchives.Publisher = book.Publisher;
                bookInArchives.DateOfPublication = book.DateOfPublication;
                deactivatedBooks.Add(bookInArchives);
            }
            ArchivesDataGrid.ItemsSource = deactivatedBooks;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddBookWindow addBookWindow = new AddBookWindow();
            if (addBookWindow.ShowDialog() == true)
            {
                BookInInventory newBook = addBookWindow.BookToBeAdded;
                booksInInventory.Add(newBook);
                InventoryDataGrid.Items.Refresh();
                MessageBox.Show
                (
                    "Book was successfully added!", "BookSeek", 
                    MessageBoxButton.OK, MessageBoxImage.Information
                );
            }
        }

        private void InventoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid dataGrid = sender as DataGrid;
                if (dataGrid != null && dataGrid.SelectedItems != null && dataGrid.SelectedItems.Count == 1)
                {
                    DataGridRow row =
                        dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.SelectedItem)
                        as DataGridRow;

                    if (row != null)
                    {
                        BookInInventory bookInInventory = row.Item as BookInInventory;
                        UpdateBookWindow updateBookWindow = new UpdateBookWindow(bookInInventory);
                        if (updateBookWindow.ShowDialog() == true) // If operation was delete
                        {
                            booksInInventory.Remove(bookInInventory);
                            deactivatedBooks.Add(bookInInventory);
                            ArchivesDataGrid.Items.Refresh();
                        }
                        InventoryDataGrid.Items.Refresh();
                    }
                }
            }
        }

        private void InventorySearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = InventorySearchTextBox.Text.ToLower();
            InventoryDataGrid.ItemsSource =
                (
                    from b in booksInInventory
                    where
                        b.Isbn.ToLower().Contains(input) ||
                        b.Title.ToLower().Contains(input) ||
                        b.Author.ToLower().Contains(input) ||
                        b.Price.ToString().Contains(input) ||
                        b.Quantity.ToString().Contains(input) ||
                        b.Publisher.ToLower().Contains(input) ||
                        b.DateOfPublication.ToString("MMMM dd, yyyy hh:mm tt").ToLower().Contains(input)
                    select b
                ).ToList();
        }

        private void InvetoryClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            InventorySearchTextBox.Text = string.Empty;
        }

        private void ArchivesSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = ArchivesSearchTextBox.Text.ToLower();
            ArchivesDataGrid.ItemsSource =
                (
                    from b in deactivatedBooks
                    where
                        b.Isbn.ToLower().Contains(input) ||
                        b.Title.ToLower().Contains(input) ||
                        b.Author.ToLower().Contains(input) ||
                        b.Price.ToString().Contains(input) ||
                        b.Quantity.ToString().Contains(input) ||
                        b.Publisher.ToLower().Contains(input) ||
                        b.DateOfPublication.ToString("MMMM dd, yyyy hh:mm tt").ToLower().Contains(input)
                    select b
                ).ToList();
        }

        private void ArchivesClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ArchivesSearchTextBox.Text = string.Empty;
        }

        private void ArchivesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid dataGrid = sender as DataGrid;
                if (dataGrid != null && dataGrid.SelectedItems != null && dataGrid.SelectedItems.Count == 1)
                {
                    DataGridRow row =
                        dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.SelectedItem)
                        as DataGridRow;

                    if (row != null)
                    {
                        BookInInventory bookInArchives= row.Item as BookInInventory;
                        MessageBoxResult answer = MessageBox.Show
                        (
                            "Reactivate product?", "Reactivate",
                             MessageBoxButton.YesNo, MessageBoxImage.Question
                        );

                        if (answer == MessageBoxResult.Yes)
                        {
                            string isbn = bookInArchives.Isbn;
                            Book book = BookOnDataGrid.GetBook(isbn, database);
                            book.BookStatus = "Active";

                            Ledger ledger = 
                                (
                                    from l in database.Ledger
                                    where 
                                        l.Isbn == isbn &&
                                        l.TrackTag == "Deactivated"
                                    select l
                                ).Single();
                            database.Ledger.Remove(ledger);
                            database.SaveChanges();

                            deactivatedBooks.Remove(bookInArchives);
                            booksInInventory.Add(bookInArchives);
                            InventoryDataGrid.Items.Refresh();
                            ArchivesDataGrid.Items.Refresh();

                            MessageBox.Show
                            (
                                "Book was successfully restored!", "BookSeek",
                                MessageBoxButton.OK, MessageBoxImage.Information
                            );
                        }
                    }
                }
            }
        }
    }
}