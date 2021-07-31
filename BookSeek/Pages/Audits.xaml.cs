using BookSeek.HelperClasses;
using BookSeek.Pages.SubPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookSeek.Windows;

namespace BookSeek.Pages
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Audits : Page
    {
        Database database = new Database();
        List<Transaction> transactions = new List<Transaction>();
        List<Track> ledgerRecords = new List<Track>();

        public Audits()
        {
            InitializeComponent();
            InitializeTallySection();
            InitializeSalesDataGrid();
            InitializeLedgerDataGrid();
        }

        private void InitializeTallySection()
        {
            DateTime startDateTime = DateTime.Today;
            DateTime endDateTime = DateTime.Today.AddDays(1);

            decimal earningsToday =
                (
                    from sale in database.Sale
                    where
                        sale.TransactionDateTime >= startDateTime &&
                        sale.TransactionDateTime < endDateTime
                    select sale.TotalAmount
                 ).ToList().Sum();
            EarningsTodayLabel.Content = earningsToday.ToString();

            int salesToday =
                (
                    from sale in database.Sale
                    where
                        sale.TransactionDateTime >= startDateTime &&
                        sale.TransactionDateTime < endDateTime
                    select sale.SaleId
                ).ToList().Count();
            SalesTodayLabel.Content = salesToday.ToString();

            var bestSeller =
                (
                    from sb in database.Sale_Book
                    group sb by new { sb.Isbn } into g
                    select new
                    {
                        g.Key.Isbn,
                        Quantity = g.Sum(sb => sb.BookQuantitySold)
                    }
                ).OrderByDescending(i => i.Quantity).First();

            string isbn = bestSeller.Isbn;
            Book book = BookOnDataGrid.GetBook(isbn, database);
            BestSellerLabel.Content = book.Title;
        }

        private void InitializeSalesDataGrid()
        {
            foreach (Sale sale in database.Sale)
            {
                Transaction transaction = new Transaction();
                transaction.SaleId = sale.SaleId;
                transaction.TransactionDateTime = sale.TransactionDateTime;

                List<Purchase> purchases = Purchase.GetPurchases(sale.SaleId, database);
                string formattedPurchases = string.Empty;
                foreach (var purchase in purchases)
                {
                    if (!string.IsNullOrEmpty(formattedPurchases))
                    {
                        formattedPurchases += $"\r\n";
                    }
                    formattedPurchases += $"{purchase.Title} x {purchase.Quantity}";
                }

                transaction.Purchases = formattedPurchases;
                if ((DateTime.Now - transaction.TransactionDateTime).TotalDays <= 7)
                {
                    transaction.ValidForReturn = true;
                }
                else
                {
                    transaction.ValidForReturn = false;
                    transaction.ToolTipMessage = "Not valid for refund (7-day Policy)";
                }
                transactions.Add(transaction);
            }
            SalesDataGrid.ItemsSource = transactions.ToList();
        }

        private void InitializeLedgerDataGrid()
        {
            foreach (Ledger ledger in database.Ledger)
            {
                string isbn = ledger.Isbn;
                Track track = new Track();
                track.TrackId = ledger.TrackId;
                track.TrackTag = ledger.TrackTag.Trim();
                track.Isbn = isbn;
                track.Title = BookOnDataGrid.GetBook(isbn, database).Title;
                track.Quantity = ledger.Quantity;
                track.EventDateTime = ledger.EventDateTime;
                ledgerRecords.Add(track);
            }
            LedgerDataGrid.ItemsSource = ledgerRecords.ToList();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int saleId = int.Parse(button.Tag.ToString());
            this.NavigationService.Navigate(new ReturnPurchase(saleId));
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int saleId = int.Parse(button.Tag.ToString());
            var sale =
                (
                    from s in database.Sale
                    where s.SaleId == saleId
                    select s
                ).Single();

            List<Purchase> purchases = new List<Purchase>();
            List<BookOnDataGrid> booksSold = new List<BookOnDataGrid>();
            purchases = Purchase.GetPurchases(saleId, database);

            foreach (var purchase in purchases)
            {
                string isbn = purchase.Isbn;
                Book bookBought = BookOnDataGrid.GetBook(isbn, database);
                BookOnDataGrid bookOnDataGrid = new BookOnDataGrid();
                bookOnDataGrid.Isbn = isbn;
                bookOnDataGrid.Item = bookBought.Title;
                bookOnDataGrid.Quantity = purchase.Quantity;
                bookOnDataGrid.Price = purchase.Price;
                bookOnDataGrid.Amount = bookOnDataGrid.Quantity * bookOnDataGrid.Price;
                booksSold.Add(bookOnDataGrid);
            }

            Receipt receipt = new Receipt();
            receipt.SaleId = saleId;
            receipt.TransactionDateTime = sale.TransactionDateTime;
            receipt.BooksSold = booksSold;
            receipt.TotalAmount = sale.TotalAmount;
            receipt.Cash = sale.Cash;
            receipt.Change = sale.Change;

            if (sale.SaleStatus.Trim() == "Standard")
            {
                ReceiptWindow receiptWindow = new ReceiptWindow(receipt);
                receiptWindow.Show();

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(receiptWindow, "Print in Progress");
                }
                receiptWindow.Close();
            }
            else
            {
                receipt.Budget = (decimal)sale.ChargePrepaid;
                receipt.AdditionalRequired = (decimal)sale.AdditionalRequired;
                ExchangeReceiptWindow exchangeReceiptWindow = new ExchangeReceiptWindow(receipt);
                exchangeReceiptWindow.Show();

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(exchangeReceiptWindow, "Print in Progress");
                }
                exchangeReceiptWindow.Close();
            }
        }

        private void SalesSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = SalesSearchTextBox.Text.ToLower();
            SalesDataGrid.ItemsSource =
                (
                    from t in transactions
                    where
                        t.SaleId.ToString().Contains(input) ||
                        t.Purchases.ToLower().Contains(input) ||
                        t.TransactionDateTime.ToString("MMMM dd, yyyy hh:mm tt").ToLower().Contains(input)
                    orderby t.SaleId
                    select t
                ).ToList();
        }

        private void SalesClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SalesSearchTextBox.Text = string.Empty;
        }

        private void LedgerSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = LedgerSearchTextBox.Text.ToLower();
            LedgerDataGrid.ItemsSource =
                (
                    from l in ledgerRecords
                    where
                        l.TrackId.ToString().Contains(input) ||
                        l.TrackTag.ToLower().Contains(input) ||
                        l.Title.ToLower().Contains(input) ||
                        l.Quantity.ToString().Contains(input) ||
                        l.EventDateTime.ToString("MMMM dd, yyyy hh:mm tt").ToLower().Contains(input)
                    orderby l.TrackId
                    select l
                ).ToList();
        }

        private void LedgerClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            LedgerSearchTextBox.Text = string.Empty;
        }
    }
}