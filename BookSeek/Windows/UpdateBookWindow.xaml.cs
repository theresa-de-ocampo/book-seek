using BookSeek.HelperClasses;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace BookSeek.Windows
{
    /// <summary>
    /// Interaction logic for UpdateBookWindow.xaml
    /// </summary>
    public partial class UpdateBookWindow : Window
    {
        Database database = new Database();
        BookInInventory bookToBeUpdated;
        string isbn, source, originalIsbn, originalSource;
        int originalQuantity;

        public UpdateBookWindow(BookInInventory bookToBeUpdated)
        {
            InitializeComponent();
            this.bookToBeUpdated = bookToBeUpdated;

            DatePublishedDatePicker.Loaded += delegate
            {
                var DatePickerTextBox = (TextBox)DatePublishedDatePicker
                    .Template
                    .FindName("PART_TextBox", DatePublishedDatePicker);
                DatePickerTextBox.Background = DatePublishedDatePicker.Background;
            };
            DatePublishedDatePicker.DisplayDateEnd = DateTime.Today;

            InitializeForm();
        }

        private void InitializeForm()
        {
            originalQuantity = bookToBeUpdated.Quantity;
            originalIsbn = isbn = bookToBeUpdated.Isbn;
            IsbnTextBox.Text = isbn;
            TitleTextBox.Text = bookToBeUpdated.Title;
            AuthorTextBox.Text = bookToBeUpdated.Author;
            PublisherTextBox.Text = bookToBeUpdated.Publisher;
            DatePublishedDatePicker.SelectedDate = bookToBeUpdated.DateOfPublication;
            PriceTextBox.Text = bookToBeUpdated.Price.ToString();
            QuantityTextBox.Text = bookToBeUpdated.Quantity.ToString();

            string relativeSource = @"\..\..\Images\Books\" + isbn + ".jpg";
            originalSource = source = Path.GetFullPath(Environment.CurrentDirectory + relativeSource);
            BitmapImage bitmapImage = ImageUtility.GetBitmapImage(source);
            BookImage.Source = bitmapImage;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string filter = "Image Files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) ";
            filter += "| *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            openFileDialog.Filter = filter;
            if (openFileDialog.ShowDialog() == true)
            {
                source = openFileDialog.FileName;
                BitmapImage bitmapImage = ImageUtility.GetBitmapImage(source);
                BookImage.Source = bitmapImage;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string isbn = IsbnTextBox.Text;
            string title = TitleTextBox.Text;
            string author = AuthorTextBox.Text;
            string priceInput = PriceTextBox.Text;
            string quantityInput = QuantityTextBox.Text;
            string publisher = PublisherTextBox.Text;
            bool dateOfPublicationFlag = true;

            try
            {
                bookToBeUpdated.DateOfPublication = DatePublishedDatePicker.SelectedDate.Value.Date;
            }
            catch (Exception)
            {
                dateOfPublicationFlag = false;
            }

            bool flag = bookToBeUpdated.SaveUserInputs
            (
                source, isbn, title, author, priceInput, quantityInput,
                publisher, dateOfPublicationFlag
            );

            if (flag)
            {
                if (originalIsbn != isbn)
                {
                    if (BookInInventory.BookAlreadyExists(isbn, database))
                    {
                        MessageBox.Show
                        (
                            "New ISBN already exists. Maybe you want to restore a deactivated book?", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error
                        );
                        return;
                    }
                }
                if (originalSource != source)
                {
                    string destinationPath =
                        Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\Images\Books\");
                    File.SetAttributes(destinationPath, FileAttributes.Normal);
                    string newSource = Path.Combine(destinationPath, bookToBeUpdated.Isbn + ".jpg");
                    File.Copy(source, newSource, true);
                    bookToBeUpdated.CoverImageSource = ImageUtility.GetBitmapImage(newSource);
                }

                int updatedQuantity = bookToBeUpdated.Quantity;
                if (originalQuantity != updatedQuantity)
                {
                    string trackTag;
                    int ledgerQuantity;

                    if (originalQuantity > updatedQuantity)
                    {
                        trackTag = "Out";
                        ledgerQuantity = originalQuantity - updatedQuantity;
                    }
                    else
                    {
                        trackTag = "In";
                        ledgerQuantity = updatedQuantity - originalQuantity;
                    }

                    Track.Add
                    (
                        trackTag, bookToBeUpdated.Isbn, bookToBeUpdated.Title, ledgerQuantity,
                        DateTime.Now, database
                    );
                }

                Book book = BookOnDataGrid.GetBook(isbn, database);
                book.Isbn = bookToBeUpdated.Isbn;
                book.Title = bookToBeUpdated.Title;
                book.Author = bookToBeUpdated.Author;
                book.Publisher = bookToBeUpdated.Publisher;
                book.DateOfPublication = bookToBeUpdated.DateOfPublication;
                book.BookQuantityOnHand = updatedQuantity;
                book.Price = bookToBeUpdated.Price;
                database.SaveChanges();
                this.Close();
                MessageBox.Show
                (
                    "Book was successfully edited!", "BookSeek",
                    MessageBoxButton.OK, MessageBoxImage.Information
                );
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult answer = MessageBox.Show
            (
                "Are you sure you want to delete this product?", "Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question
            );

            if (answer == MessageBoxResult.Yes)
            {
                Book book = BookOnDataGrid.GetBook(isbn, database);
                book.BookStatus = "Deleted";
                Track.Add
                (
                    "Deactivated", book.Isbn, book.Title, book.BookQuantityOnHand,
                    DateTime.Now, database
                );
                database.SaveChanges();

                this.DialogResult = true; // Flag to indicate that the operation executed was delete.
                this.Close();
                MessageBox.Show
                (
                    "Book was successfully deactivated!", "BookSeek",
                    MessageBoxButton.OK, MessageBoxImage.Information
                );
            }
        }
    }
}