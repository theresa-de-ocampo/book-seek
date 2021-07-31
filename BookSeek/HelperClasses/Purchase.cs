using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSeek.HelperClasses
{
    public class Purchase
    {
        #pragma warning disable 0169
        string isbn;
        string title;
        int quantity;

        public string Isbn { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        
        public static List<Purchase> GetPurchases(int saleId, Database database)
        {
            var purchases =
                (
                    from sale in database.Sale
                    join sale_book in database.Sale_Book on sale.SaleId equals sale_book.SaleId
                    join book in database.Book on sale_book.Isbn equals book.Isbn
                    where sale.SaleId == saleId
                    orderby book.Title
                    select new Purchase
                    {
                        Isbn = book.Isbn,
                        Title = book.Title,
                        Quantity = sale_book.BookQuantitySold,
                        Price = sale_book.Price
                    }
                ).ToList();
            return purchases as List<Purchase>;
        }

        public static bool Equal(List<Purchase> purchases, List<BookOnDataGrid> checkedItems)
        {
            if (purchases.Count != checkedItems.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < purchases.Count; ++i)
                {
                    if (purchases[i].Quantity != checkedItems[i].Quantity)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
