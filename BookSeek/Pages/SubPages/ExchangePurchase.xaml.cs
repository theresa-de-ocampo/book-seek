using BookSeek.HelperClasses;
using BookSeek.Windows;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;

namespace BookSeek.Pages.SubPages
{
    /// <summary>
    /// Interaction logic for ExchangePurchase.xaml
    /// </summary>
    public partial class ExchangePurchase : Page
    {
        int saleId;
        decimal budget;
        List<Purchase> purchases;
        List<BookOnDataGrid> checkedItems;

        Database database = new Database();
        List<Book> books = new List<Book>();
        List<BookOnDataGrid> booksOnDataGrid = new List<BookOnDataGrid>();

        string caption = "Error";
        MessageBoxButton messageBoxButton = MessageBoxButton.OK;
        MessageBoxImage messageBoxImage = MessageBoxImage.Error;

        
        public ExchangePurchase
            (int saleId, decimal budget, List<Purchase> purchases, List<BookOnDataGrid> checkedItems)
        {
            InitializeComponent();
            this.saleId = saleId;
            this.budget = budget;
            this.purchases = purchases;
            this.checkedItems = checkedItems;
            InitializeExchangePurchaseDataGrid();
            BudgetTextBox.Text = budget.ToString();
        }

        private void UpdateComputationSection()
        {
            decimal total = 0;

            foreach (var bookOnDataGrid in booksOnDataGrid)
            {
                if (bookOnDataGrid.Selected)
                {
                    total += bookOnDataGrid.Amount;
                }
            }

            TotalAmountTextBox.Text = total.ToString();

            if (total > budget)
            {
                AdditionalRequiredTextBox.Text = (total - budget).ToString();
                CashTextBox.IsEnabled = true;
                ChangeDueTextBox.Text = "0";
            }
            else
            {
                AdditionalRequiredTextBox.Text = "0";
                CashTextBox.Text = "0";
                CashTextBox.IsEnabled = false;
                ChangeDueTextBox.Text = (budget - total).ToString();
            }
        }
            
        private void InitializeExchangePurchaseDataGrid()
        {
            books = BookOnDataGrid.GetBooksWithStock(database);
            foreach (var book in books)
            {
                BookOnDataGrid bookOnDataGrid = new BookOnDataGrid();
                bookOnDataGrid.Isbn = book.Isbn;
                bookOnDataGrid.Item = book.Title;
                bookOnDataGrid.Quantity = 1;
                bookOnDataGrid.Price = book.Price;
                bookOnDataGrid.Amount = bookOnDataGrid.Quantity * book.Price;
                booksOnDataGrid.Add(bookOnDataGrid);
            }
            ExchangePurchaseDataGrid.ItemsSource = booksOnDataGrid.ToList();
        }

        private void ExchangePurchaseDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                int rowIndex = e.Row.GetIndex();
                TextBox quantityTextBox = e.EditingElement as TextBox;
                if (quantityTextBox != null)
                {
                    int newQuantity = int.Parse((e.EditingElement as TextBox).Text);
                    BookOnDataGrid bookOnDataGridToBeEdited = booksOnDataGrid[rowIndex];
                    string isbn = bookOnDataGridToBeEdited.Isbn;
                    Book book = BookOnDataGrid.GetBook(isbn, database);
                    int stock = book.BookQuantityOnHand;

                    if (newQuantity < 0)
                    {
                        throw new Exception();
                    }
                    else if (newQuantity == 0)
                    {
                        if (bookOnDataGridToBeEdited.Selected)
                        {
                            bookOnDataGridToBeEdited.Selected = false;
                            MessageBox.Show
                            (
                                "Item was deselected.",
                                "BookSeek", messageBoxButton, MessageBoxImage.Information
                            );
                        }
                        else
                        {
                            MessageBox.Show
                            (
                                "Item is already unselected.",
                                "BookSeek", messageBoxButton, MessageBoxImage.Information
                            );
                        }
                        ExchangePurchaseDataGrid.ItemsSource = booksOnDataGrid.ToList();
                    }
                    else
                    {
                        if (stock < newQuantity)
                        {
                            MessageBox.Show
                            (
                                $"Sorry, there are only {stock} stocks left.",
                                caption, messageBoxButton, messageBoxImage
                            );
                            bookOnDataGridToBeEdited.Quantity = stock;
                        }
                        else
                        {
                            bookOnDataGridToBeEdited.Selected = true;
                            bookOnDataGridToBeEdited.Quantity = newQuantity;
                        }
                        bookOnDataGridToBeEdited.Amount =
                            bookOnDataGridToBeEdited.Quantity * bookOnDataGridToBeEdited.Price;
                        ExchangePurchaseDataGrid.ItemsSource = booksOnDataGrid.ToList();
                        UpdateComputationSection();
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a number.", caption, messageBoxButton, messageBoxImage);
            }
            catch (Exception)
            {
                ExchangePurchaseDataGrid.ItemsSource = booksOnDataGrid.ToList();
                MessageBox.Show("Invalid quantity input!", caption, messageBoxButton, messageBoxImage);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            int rowIndex = BookOnDataGrid.GetRowIndex(sender, e);
            booksOnDataGrid[rowIndex].Selected = true;
            UpdateComputationSection();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            int rowIndex = BookOnDataGrid.GetRowIndex(sender, e);
            booksOnDataGrid[rowIndex].Selected = false;
            UpdateComputationSection();
        }

        private void CashTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(CashTextBox.Text))
            {
                try
                {
                    decimal cash = decimal.Parse(CashTextBox.Text);
                    if (cash < 0)
                    {
                        throw new Exception();
                    }
                    decimal changeDue = cash - decimal.Parse(AdditionalRequiredTextBox.Text);
                    ChangeDueTextBox.Text = changeDue.ToString();
                }
                catch (Exception)
                {
                    CashTextBox.Text = ChangeDueTextBox.Text = String.Empty;
                    MessageBox.Show("Invalid cash input!", caption, messageBoxButton, messageBoxImage);
                }
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(CashTextBox.Text))
            {
                MessageBox.Show("Please enter the cash amount.", caption, messageBoxButton, messageBoxImage);
            }
            else if (decimal.Parse(CashTextBox.Text) < decimal.Parse(AdditionalRequiredTextBox.Text))
            {
                MessageBox.Show
                (
                    "Sorry, you don't have sufficient money to proceed with this transaction.",
                    caption, messageBoxButton, messageBoxImage
                );
            }
            else
            {
                MessageBoxResult answer = MessageBox.Show
                (
                    "Restock inventory?",
                    "Cash Back",
                     MessageBoxButton.YesNoCancel,
                     MessageBoxImage.Question
                );

                if (Purchase.Equal(purchases, checkedItems))
                {
                    ReturnUtility.FullRefund(saleId, database);
                }
                else
                {
                    ReturnUtility.PatialRefund(saleId, checkedItems, database);
                }

                decimal budget = decimal.Parse(BudgetTextBox.Text);
                decimal totalAmount = decimal.Parse(TotalAmountTextBox.Text);
                decimal additionalRequired = decimal.Parse(AdditionalRequiredTextBox.Text);
                decimal cash = decimal.Parse(CashTextBox.Text);
                decimal change = decimal.Parse(ChangeDueTextBox.Text);

                Sale sale = new Sale()
                {
                    TransactionDateTime = DateTime.Now,
                    SaleStatus = "Exchanged",
                    TotalAmount = totalAmount,
                    Cash = cash,
                    Change = change,
                    ChargePrepaid = budget,
                    AdditionalRequired = additionalRequired
                };
                database.Sale.Add(sale);
                database.SaveChanges();
                int newSaleId = sale.SaleId;
                DateTime transactionDateTime = sale.TransactionDateTime;
                List<BookOnDataGrid> selectedBooks = new List<BookOnDataGrid>();

                foreach (var bookOnDataGrid in booksOnDataGrid)
                {
                    if (bookOnDataGrid.Selected)
                    {
                        selectedBooks.Add(bookOnDataGrid);
                        string isbn = bookOnDataGrid.Isbn;
                        int quantity = bookOnDataGrid.Quantity;
                        Sale_Book sale_Book = new Sale_Book()
                        {
                            SaleId = newSaleId,
                            Isbn = isbn,
                            BookQuantitySold = quantity,
                            Price = bookOnDataGrid.Price
                        };
                        Book book = BookOnDataGrid.GetBook(isbn, database);
                        book.BookQuantityOnHand -= quantity;
                        database.Sale_Book.Add(sale_Book);
                        database.SaveChanges();
                    }
                }

                Receipt receipt = new Receipt();
                receipt.SaleId = newSaleId;
                receipt.TransactionDateTime = transactionDateTime;
                receipt.BooksSold = selectedBooks;
                receipt.Budget = budget;
                receipt.TotalAmount = totalAmount;
                receipt.AdditionalRequired = additionalRequired;
                receipt.Cash = cash;
                receipt.Change = change;
                ExchangeReceiptWindow exchangeReceiptWindow = new ExchangeReceiptWindow(receipt);
                exchangeReceiptWindow.Show();

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(exchangeReceiptWindow, "Print in Progress");
                }
                exchangeReceiptWindow.Close();

                string message = "Exchange was successfully processed!";
                ReturnUtility.RestockInventory(answer, checkedItems, message, database);
                this.NavigationService.Navigate(new Uri("Pages/Home.xaml", UriKind.Relative));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult answer = MessageBox.Show
                (
                    "Are you sure you want to cancel? Any progress will be lost.", "BookSeek", 
                     MessageBoxButton.YesNo, MessageBoxImage.Question
                );
            if (answer == MessageBoxResult.Yes)
            {
                this.NavigationService.Navigate(new Uri("Pages/Home.xaml", UriKind.Relative));
            }
        }

        private void ExchangePurchaseSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = ExchangePurchaseSearchTextBox.Text.ToLower();
            ExchangePurchaseDataGrid.ItemsSource =
                (
                    from b in booksOnDataGrid
                    where
                        b.Isbn.ToLower().Contains(input) ||
                        b.Item.ToLower().Contains(input) ||
                        b.Quantity.ToString().Contains(input) ||
                        b.Price.ToString().Contains(input) ||
                        b.Amount.ToString().Contains(input)
                    select b
                ).ToList();
        }

        private void ExchangePurchaseClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ExchangePurchaseSearchTextBox.Text = string.Empty;
        }
    }
}