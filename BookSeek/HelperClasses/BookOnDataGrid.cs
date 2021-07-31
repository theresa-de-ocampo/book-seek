using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookSeek.HelperClasses
{
    public class BookOnDataGrid
    {
        #pragma warning disable 0169
        bool selected;
        string isbn;
        string item;
        int quantity;
        decimal price;
        decimal amount;

        public bool Selected { get; set; }
        public string Isbn { get; set; }
        public string Item { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }

        public static Book GetBook(string isbn, Database database)
        {
            return
                (
                    from book in database.Book
                    where book.Isbn == isbn
                    select book
                ).Single();
        }

        public static List<Book> GetAllBooks(Database database)
        {
            return
                (
                    from book in database.Book
                    orderby book.Title
                    select book
                ).ToList();
        }

        public static List<Book> GetActiveBooks(Database database)
        {
            return
                (
                    from book in database.Book
                    orderby book.Title
                    where book.BookStatus == "Active"
                    select book
                ).ToList();
        }

        public static List<Book> GetDeactivatedBooks(Database database)
        {
            return
                (
                    from book in database.Book
                    orderby book.Title
                    where book.BookStatus == "Deleted"
                    select book
                ).ToList();
        }

        public static List<Book> GetBooksWithStock(Database database)
        {
            return
                (
                    from book in database.Book
                    orderby book.Title
                    where book.BookQuantityOnHand > 0
                    select book
                ).ToList();
        }

        public static int GetRowIndex(object sender, RoutedEventArgs e)
        {
            DataGridCell checkBoxCell = sender as DataGridCell;
            DataGridRow row = DataGridRow.GetRowContainingElement(checkBoxCell);
            return row.GetIndex();
        }
    }
}
