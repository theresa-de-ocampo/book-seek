using BookSeek.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookSeek.Pages.SubPages
{
    /// <summary>
    /// Interaction logic for ReturnPurchase.xaml
    /// </summary>
    public partial class ReturnPurchase : Page
    {
        int saleId;
        Database database = new Database();
        List<Purchase> purchases = new List<Purchase>();
        List<BookOnDataGrid> booksOnDataGrid = new List<BookOnDataGrid>();

        string caption = "Error";
        MessageBoxButton messageBoxButton = MessageBoxButton.OK;
        MessageBoxImage messageBoxImage = MessageBoxImage.Error;

        public ReturnPurchase(int saleId)
        {
            InitializeComponent();
            this.saleId = saleId;
            InitializeReturnPurchaseDataGrid();
        }

        private void UpdateTotal()
        {
            decimal total = 0;

            foreach (var bookOnDataGrid in booksOnDataGrid)
            {
                if (bookOnDataGrid.Selected)
                {
                    total += bookOnDataGrid.Amount;
                }
            }
            TotalLabel.Content = total.ToString();
        }

        private void InitializeReturnPurchaseDataGrid()
        {
            purchases = Purchase.GetPurchases(saleId, database);
            foreach (var purchase in purchases)
            {
                string isbn = purchase.Isbn;
                Book bookBought = BookOnDataGrid.GetBook(isbn, database);
                BookOnDataGrid bookOnDataGrid = new BookOnDataGrid();
                bookOnDataGrid.Isbn = isbn;
                bookOnDataGrid.Item = bookBought.Title;
                bookOnDataGrid.Quantity = purchase.Quantity;
                bookOnDataGrid.Price = bookBought.Price;
                bookOnDataGrid.Amount = bookOnDataGrid.Quantity * bookOnDataGrid.Price;
                booksOnDataGrid.Add(bookOnDataGrid);
            }

            if (booksOnDataGrid.Count == 1)
            {
                booksOnDataGrid[0].Selected = true;
            }
            ReturnPurchaseDataGrid.ItemsSource = booksOnDataGrid.ToList();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            int rowIndex = BookOnDataGrid.GetRowIndex(sender, e);
            booksOnDataGrid[rowIndex].Selected = true;
            UpdateTotal();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            int rowIndex = BookOnDataGrid.GetRowIndex(sender, e);
            booksOnDataGrid[rowIndex].Selected = false;
            UpdateTotal();
        }

        private void ReturnPurchaseDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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
                    int quantityBought = purchases[rowIndex].Quantity;

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
                        ReturnPurchaseDataGrid.ItemsSource = booksOnDataGrid.ToList();
                    }
                    else
                    {
                        if (newQuantity > quantityBought)
                        {
                            MessageBox.Show
                                (
                                    $"Quantity entered is greater than quantity bought ({quantityBought}).",
                                    caption, messageBoxButton, messageBoxImage
                                );
                            bookOnDataGridToBeEdited.Quantity = quantityBought;
                        }
                        else
                        {
                            bookOnDataGridToBeEdited.Selected = true;
                            bookOnDataGridToBeEdited.Quantity = newQuantity;
                        }
                        bookOnDataGridToBeEdited.Amount = 
                            bookOnDataGridToBeEdited.Quantity * bookOnDataGridToBeEdited.Price;
                        ReturnPurchaseDataGrid.ItemsSource = booksOnDataGrid.ToList();
                        UpdateTotal();
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a number.", caption, messageBoxButton, messageBoxImage);
            }
            catch (Exception)
            {
                ReturnPurchaseDataGrid.ItemsSource = booksOnDataGrid.ToList();
                MessageBox.Show("Invalid quantity input!", caption, messageBoxButton, messageBoxImage);
            }
        }

        private void ExchangeItemsButton_Click(object sender, RoutedEventArgs e)
        {
            List<BookOnDataGrid> checkedItems = ReturnUtility.GetCheckedItems(booksOnDataGrid);
            if (checkedItems.Count > 0)
            {
                decimal budget = decimal.Parse(TotalLabel.Content.ToString());
                this.NavigationService.Navigate(new ExchangePurchase(saleId, budget, purchases, checkedItems));
            }
            else
            {
                MessageBox.Show("You haven't selected any item to return!", caption, messageBoxButton, messageBoxImage);
            }
        }

        private void CashBackButton_Click(object sender, RoutedEventArgs e)
        {
            List<BookOnDataGrid> checkedItems = ReturnUtility.GetCheckedItems(booksOnDataGrid);
            if (checkedItems.Count > 0)
            {
                MessageBoxResult answer = MessageBox.Show
                (
                    "Restock inventory?",
                    "Cash Back",
                     MessageBoxButton.YesNoCancel,
                     MessageBoxImage.Question
                );

                if (answer != MessageBoxResult.Cancel)
                {
                    if (Purchase.Equal(purchases, checkedItems))
                    {
                        ReturnUtility.FullRefund(saleId, database);
                    }
                    else
                    {
                        ReturnUtility.PatialRefund(saleId, checkedItems, database);
                    }

                    string message = "Refund was successfully processed!";
                    ReturnUtility.RestockInventory(answer, checkedItems, message, database);
                    this.NavigationService.Navigate(new Uri("Pages/Home.xaml", UriKind.Relative));
                }
            }
            else
            {
                MessageBox.Show
                    (
                        "You haven't selected any item to return!", 
                        caption, messageBoxButton, messageBoxImage
                    );
            }
        }
    }
}
