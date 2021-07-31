using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BookSeek.HelperClasses
{
    public class BookInInventory
    {
        #pragma warning disable 0169
        private BitmapImage coverImageSource;
        private string isbn;
        private string title;
        private string author;
        private decimal price;
        private int quantity;
        private string publisher;
        private DateTime dateOfPublication;

        private string caption = "Error";
        private MessageBoxButton messageBoxButton = MessageBoxButton.OK;
        private MessageBoxImage messageBoxImage = MessageBoxImage.Error;

        public BitmapImage CoverImageSource { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Publisher { get; set; }
        public DateTime DateOfPublication { get; set; }

        public bool SaveUserInputs
            (
                string coverImageSource, string isbn, string title, string author, string priceInput, 
                string quantityInput, string publisher, bool dateOfPublicationFlag
            )
        {
            bool priceFlag = true, quantityFlag = true;

            try
            {
                Price = decimal.Parse(priceInput);
            }
            catch (Exception)
            {
                priceFlag = false;
            }

            try
            {
                Quantity = int.Parse(quantityInput);
            }
            catch (Exception)
            {
                quantityFlag = false;
            }

            if
                (
                    !String.IsNullOrEmpty(isbn) && 
                    !String.IsNullOrEmpty(title) && !String.IsNullOrEmpty(author) &&
                    !String.IsNullOrEmpty(priceInput) && !String.IsNullOrEmpty(quantityInput) && 
                    !String.IsNullOrEmpty(publisher)
                )
            {
                if (!priceFlag || price < 0)
                {
                    MessageBox.Show("Invalid price input!", caption, messageBoxButton, messageBoxImage);
                    return false;
                }
                if (!quantityFlag || quantity < 0)
                {
                    MessageBox.Show("Invalid quantity input!", caption, messageBoxButton, messageBoxImage);
                    return false;
                }
                if (!dateOfPublicationFlag)
                {
                    MessageBox.Show("Invalid date input!", caption, messageBoxButton, messageBoxImage);
                    return false;
                }
                if (String.IsNullOrEmpty(coverImageSource))
                {
                    MessageBox.Show
                        ("Please upload a cover image.", caption, messageBoxButton, messageBoxImage);
                    return false;
                }

                // If all inputs are valid, set all the values of the properties.
                // [NOTE]: Checking of `DateOfPublication` should be done in the calling module.
                Isbn = isbn;
                Title = title;
                Author = author;
                Publisher = publisher;
                return true;
            }
            else
            {
                MessageBox.Show
                    ("Please fill out all required fields.", caption, messageBoxButton, messageBoxImage);
                return false;
            }
        }

        public static bool BookAlreadyExists(string isbn, Database database)
        {
            List<Book> books = BookOnDataGrid.GetAllBooks(database);
            return books.Any(book => book.Isbn == isbn);
            return false;
        }
    }
}
