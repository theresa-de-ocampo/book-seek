using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookSeek.HelperClasses
{
    public class ReturnUtility
    {
        public static List<BookOnDataGrid> GetCheckedItems(List<BookOnDataGrid> booksOnDataGrid)
        {
            List<BookOnDataGrid> checkedItems = new List<BookOnDataGrid>();
            foreach (var bookOnDataGrid in booksOnDataGrid)
            {
                if (bookOnDataGrid.Selected)
                {
                    checkedItems.Add(bookOnDataGrid);
                }
            }
            return checkedItems;
        }

        public static void FullRefund(int saleId, Database database)
        {
            // Delete transaction from sales history
            var sale_book =
                (
                    from sb in database.Sale_Book
                    where sb.SaleId == saleId
                    select sb
                ).FirstOrDefault();
            database.Sale_Book.Remove(sale_book);
            database.SaveChanges();

            var sale =
                (
                    from s in database.Sale
                    where s.SaleId == saleId
                    select s
                ).Single();
            database.Sale.Remove(sale);
            database.SaveChanges();
        }

        public static void PatialRefund(int saleId, List<BookOnDataGrid> checkedItems, Database database)
        {
            // Update this instance of transaction from the sales history.
            foreach (var checkedItem in checkedItems)
            {
                var sale_book =
                    (
                        from sb in database.Sale_Book
                        where sb.Isbn == checkedItem.Isbn && sb.SaleId == saleId
                        select sb
                    ).Single();
                sale_book.BookQuantitySold -= checkedItem.Quantity;

                if (sale_book.BookQuantitySold == 0)
                {
                    database.Sale_Book.Remove(sale_book);
                }

                database.SaveChanges();
            }
        }

        public static void RestockInventory
            (
                MessageBoxResult answer, List<BookOnDataGrid> checkedItems, 
                string message, Database database
            )
        {
            if (answer == MessageBoxResult.Yes)
            {
                foreach (var checkedItem in checkedItems)
                {
                    Book bookReturned = BookOnDataGrid.GetBook(checkedItem.Isbn, database);
                    bookReturned.BookQuantityOnHand += checkedItem.Quantity;
                    database.SaveChanges();
                }
            }
            else
            {
                foreach (var checkedItem in checkedItems)
                {
                    Book bookReturned = BookOnDataGrid.GetBook(checkedItem.Isbn, database);
                    Track.Add
                    (
                        "Out", bookReturned.Isbn, bookReturned.Title, checkedItem.Quantity,
                        DateTime.Now, database
                    );
                }
            }
            MessageBox.Show
            (
                message,
                "BookSeek", MessageBoxButton.OK, MessageBoxImage.Information
            );
        }
    }
}
