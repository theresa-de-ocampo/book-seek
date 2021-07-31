using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using BookSeek.HelperClasses;
using Microsoft.Win32;

namespace BookSeek.Windows
{
    /// <summary>
    /// Interaction logic for AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        Database database = new Database();
        BookInInventory bookToBeAdded = new BookInInventory();
        string source;

        public BookInInventory BookToBeAdded { get { return bookToBeAdded; } }

        public AddBookWindow()
        {
            InitializeComponent();
            DatePublishedDatePicker.Loaded += delegate
            {
                var DatePickerTextBox = (TextBox)DatePublishedDatePicker
                    .Template
                    .FindName("PART_TextBox", DatePublishedDatePicker);
                DatePickerTextBox.Background = DatePublishedDatePicker.Background;
            };
            DatePublishedDatePicker.DisplayDateEnd = DateTime.Today;

            BitmapImage bitmapImage = ImageUtility.GetBitmapImage
                (
                    Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\Images\Books\default.jpg")
                );
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
                bookToBeAdded.DateOfPublication = DatePublishedDatePicker.SelectedDate.Value.Date;
            }
            catch (Exception)
            {
                dateOfPublicationFlag = false;
            }
            
            bool flag = bookToBeAdded.SaveUserInputs
            (
                source, isbn, title, author, priceInput, quantityInput,
                publisher, dateOfPublicationFlag
            );

            if (flag)
            {
                if (BookInInventory.BookAlreadyExists(isbn, database))
                {
                    MessageBox.Show
                    (
                        "ISBN already exists. Maybe you want to restore a deactivated book?", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error
                    );
                }
                else
                {
                    string destinationPath =
                    Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\Images\Books\");
                    File.SetAttributes(destinationPath, FileAttributes.Normal);
                    string newSource = Path.Combine(destinationPath, isbn + ".jpg");
                    File.Copy(source, newSource, true);
                    bookToBeAdded.CoverImageSource = ImageUtility.GetBitmapImage(newSource);

                    Book book = new Book
                    {
                        Isbn = bookToBeAdded.Isbn,
                        Title = bookToBeAdded.Title,
                        Author = bookToBeAdded.Author,
                        Publisher = bookToBeAdded.Publisher,
                        DateOfPublication = bookToBeAdded.DateOfPublication,
                        BookQuantityOnHand = bookToBeAdded.Quantity,
                        Price = bookToBeAdded.Price,
                        BookStatus = "Active"
                    };

                    database.Book.Add(book);
                    database.SaveChanges();
                    Track.Add
                    (
                        "In", bookToBeAdded.Isbn, bookToBeAdded.Title, bookToBeAdded.Quantity,
                        DateTime.Now, database
                    );
                    this.DialogResult = true;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
