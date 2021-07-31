using BookSeek.HelperClasses;
using BookSeek.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;

namespace BookSeek.Pages
{
    /// <summary>
    /// Interaction logic for Pos.xaml
    /// </summary>
    public partial class Pos : Page
    {
        Database database = new Database();
        List<Book> books = new List<Book>();
        List<BookOnDataGrid> booksOnDataGrid = new List<BookOnDataGrid>();
        List<Button> addOrVoidButtons = new List<Button>();

        string caption = "Error";
        MessageBoxButton messageBoxButton = MessageBoxButton.OK;
        MessageBoxImage messageBoxImage = MessageBoxImage.Error;

        public Pos()
        {
            InitializeComponent();
            books = BookOnDataGrid.GetActiveBooks(database);
            SetBooksSection();
        }

        private void UpdateTotalAmount()
        {
            decimal totalAmount = 0;
            foreach (var book in booksOnDataGrid)
            {
                totalAmount += book.Amount;
            }
            TotalAmountTextBox.Text = totalAmount.ToString();
        }

        private void UpdateComputationSection()
        {
            UpdateTotalAmount();
            CashTextBox_LostFocus(new object(), new RoutedEventArgs());
        }

        private void SetBooksSection()
        {
            addOrVoidButtons.Clear();
            BooksUniformGrid.Children.Clear();

            void DetailsButton_Click(object sender, RoutedEventArgs e)
            {
                //MessageBox.Show("Details");
            }

            if (books.Count <= 0)
            {
                BooksUniformGrid.Columns = 1;
                TextBlock resultMessage = new TextBlock();
                resultMessage.Text = "No books found.";
                resultMessage.Style = this.FindResource("ResultTextBlock") as Style;
                resultMessage.TextWrapping = TextWrapping.Wrap;
                BooksUniformGrid.Children.Add(resultMessage);
            }
            else
            {
                BooksUniformGrid.Columns = 4;
                foreach (var book in books)
                {
                    Grid bookContainer = new Grid();
                    bookContainer.Margin = new Thickness(15, 25, 15, 25);
                    RowDefinition rowDef0 = new RowDefinition();
                    RowDefinition rowDef1 = new RowDefinition();
                    RowDefinition rowDef2 = new RowDefinition();
                    rowDef0.Height = new GridLength(150, GridUnitType.Pixel);
                    rowDef1.Height = new GridLength(85, GridUnitType.Pixel);
                    rowDef2.Height = new GridLength(0.5, GridUnitType.Auto);
                    bookContainer.RowDefinitions.Add(rowDef0);
                    bookContainer.RowDefinitions.Add(rowDef1);
                    bookContainer.RowDefinitions.Add(rowDef2);

                    Button detailsButton = new Button();
                    detailsButton.Click += DetailsButton_Click;
                    detailsButton.Tag = book.Isbn;
                    string path = Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\Images\Books\");
                    string imageSource = path + book.Isbn + ".jpg";
                    Image bookCover = ImageUtility.GetImage(imageSource);
                    detailsButton.Content = bookCover;
                    Grid.SetRow(detailsButton, 0);

                    TextBlock bookTitleTextBlock = new TextBlock();
                    bookTitleTextBlock.Text = book.Title;
                    bookTitleTextBlock.TextWrapping = TextWrapping.Wrap;
                    bookTitleTextBlock.Style = this.FindResource("BookTitleTextBlock") as Style;
                    Grid.SetRow(bookTitleTextBlock, 1);

                    Button addOrVoidButton = new Button();
                    addOrVoidButton.Content = "ADD";
                    addOrVoidButton.Click += AddOrVoidButton_Click;
                    addOrVoidButton.Tag = book.Isbn;
                    addOrVoidButton.Style = this.FindResource("SecondaryButton") as Style;
                    addOrVoidButtons.Add(addOrVoidButton);
                    Grid.SetRow(addOrVoidButton, 2);

                    if (book.BookQuantityOnHand <= 0)
                    {
                        addOrVoidButton.IsEnabled = false;
                        addOrVoidButton.ToolTip = "Out of Stock!";
                    }

                    bookContainer.Children.Add(detailsButton);
                    bookContainer.Children.Add(bookTitleTextBlock);
                    bookContainer.Children.Add(addOrVoidButton);
                    BooksUniformGrid.Children.Add(bookContainer);
                }
            }
        }

        void AddOrVoidButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string isbn = button.Tag.ToString();
            string action = button.Content.ToString();

            if (action == "ADD")
            {
                Book item = GetBook(isbn);
                BookOnDataGrid bookOnDataGrid = new BookOnDataGrid();
                bookOnDataGrid.Isbn = item.Isbn;
                bookOnDataGrid.Item = item.Title;
                bookOnDataGrid.Quantity = 1;
                bookOnDataGrid.Price = item.Price;
                bookOnDataGrid.Amount = bookOnDataGrid.Quantity * bookOnDataGrid.Price;
                booksOnDataGrid.Add(bookOnDataGrid);

                TransactionDataGrid.ItemsSource = booksOnDataGrid.ToList();
                button.Content = "VOID";
                button.BorderBrush = (Brush)this.FindResource("PrimaryRedBrush");
            }
            else if (action == "VOID")
            {
                MessageBoxResult answer = MessageBox.Show
                (
                    "Are you sure you want to void this item?", "Void",
                     MessageBoxButton.YesNo, MessageBoxImage.Question
                );

                if (answer == MessageBoxResult.Yes)
                {
                    booksOnDataGrid.RemoveAll(book => book.Isbn == isbn);
                    TransactionDataGrid.ItemsSource = booksOnDataGrid.ToList();
                    button.Content = "ADD";
                    button.BorderBrush = (Brush)this.FindResource("TertiaryBlueBrush");
                }
                else
                {
                    TransactionDataGrid.ItemsSource = booksOnDataGrid.ToList();
                }
            }
            else
            {
                MessageBox.Show("Sorry, an unexpected error occurred.", caption, messageBoxButton, messageBoxImage);
            }

            UpdateComputationSection();
        }

        private void TransactionDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                int rowIndex = e.Row.GetIndex();
                int newQuantity = int.Parse((e.EditingElement as TextBox).Text);
                BookOnDataGrid bookOnDataGridToBeEdited = booksOnDataGrid[rowIndex];
                string isbn = bookOnDataGridToBeEdited.Isbn;
                Book book = GetBook(isbn);
                int stock = book.BookQuantityOnHand;

                if (newQuantity < 0)
                {
                    throw new Exception();
                }
                else if (newQuantity == 0)
                {
                    foreach (Button button in addOrVoidButtons)
                    {
                        if (button.Tag.ToString() == isbn)
                        {
                            AddOrVoidButton_Click(button, new RoutedEventArgs());
                        }
                    }
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
                        bookOnDataGridToBeEdited.Quantity = newQuantity;
                    }
                    bookOnDataGridToBeEdited.Amount = 
                        bookOnDataGridToBeEdited.Quantity * bookOnDataGridToBeEdited.Price;
                    TransactionDataGrid.ItemsSource = booksOnDataGrid.ToList();
                    UpdateComputationSection();
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a number.", caption, messageBoxButton, messageBoxImage);
            }
            catch (Exception)
            {
                TransactionDataGrid.ItemsSource = booksOnDataGrid.ToList();
                MessageBox.Show("Invalid quantity input!", caption, messageBoxButton, messageBoxImage);
            }
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
                    decimal changeDue = cash - decimal.Parse(TotalAmountTextBox.Text);
                    ChangeDueTextBox.Text = changeDue.ToString();
                }
                catch(Exception)
                {
                    CashTextBox.Text = ChangeDueTextBox.Text = String.Empty;
                    MessageBox.Show("Invalid cash input!", caption, messageBoxButton, messageBoxImage);
                }
            }
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (booksOnDataGrid.Count > 0)
            {
                if (String.IsNullOrEmpty(CashTextBox.Text))
                {
                    MessageBox.Show("Please enter the cash amount.", caption, messageBoxButton, messageBoxImage);
                }
                else if (decimal.Parse(CashTextBox.Text) < decimal.Parse(TotalAmountTextBox.Text))
                {
                    MessageBox.Show
                    (
                        "Sorry, you don't have sufficient money to proceed with this transaction.",
                        caption, messageBoxButton, messageBoxImage
                    );
                }
                else
                {
                    decimal totalAmount = decimal.Parse(TotalAmountTextBox.Text);
                    decimal cash = decimal.Parse(CashTextBox.Text);
                    decimal change = decimal.Parse(ChangeDueTextBox.Text);
                    Sale sale = new Sale() 
                    { 
                        TransactionDateTime = DateTime.Now,
                        SaleStatus = "Standard",
                        TotalAmount = totalAmount,
                        Cash = cash,
                        Change = change
                    };
                    database.Sale.Add(sale);
                    database.SaveChanges();
                    int saleId = sale.SaleId;
                    DateTime transactionDateTime = sale.TransactionDateTime;

                    foreach (var bookOnDataGrid in booksOnDataGrid)
                    {
                        string isbn = bookOnDataGrid.Isbn;
                        int quantity = bookOnDataGrid.Quantity;
                        Sale_Book sale_Book = new Sale_Book()
                        {
                            SaleId = saleId,
                            Isbn = isbn,
                            BookQuantitySold = quantity,
                            Price = bookOnDataGrid.Price
                        };
                        Book book = GetBook(isbn);
                        book.BookQuantityOnHand -= quantity;
                        database.Sale_Book.Add(sale_Book);
                        database.SaveChanges();
                    }
                    Receipt receipt = new Receipt();
                    receipt.SaleId = saleId;
                    receipt.TransactionDateTime = transactionDateTime;
                    receipt.BooksSold = booksOnDataGrid;
                    receipt.TotalAmount = totalAmount;
                    receipt.Cash = cash;
                    receipt.Change = change;
                    ReceiptWindow receiptWindow = new ReceiptWindow(receipt);
                    receiptWindow.Show();

                    PrintDialog printDialog = new PrintDialog();
                    if (printDialog.ShowDialog() == true)
                    {
                        printDialog.PrintVisual(receiptWindow, "Print in Progress");
                    }
                    receiptWindow.Close();
                    CancelButton_Click(new object(), new RoutedEventArgs());
                }
            }
            else
            {
                MessageBox.Show
                    (
                        "Add items to cart.",
                        caption, messageBoxButton, messageBoxImage
                    );
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TotalAmountTextBox.Text = CashTextBox.Text = ChangeDueTextBox.Text = String.Empty;
            booksOnDataGrid.Clear();
            TransactionDataGrid.ItemsSource = booksOnDataGrid.ToList();
            foreach (Button button in addOrVoidButtons)
            {
                button.Content = "ADD";
                button.BorderBrush = (Brush)this.FindResource("TertiaryBlueBrush");
            }
        }

        public Book GetBook(string isbn)
        {
            return
                (
                    from book in database.Book
                    where book.Isbn == isbn
                    select book
                ).Single();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = SearchTextBox.Text.ToLower();
            books = 
                (
                    from book in database.Book
                    orderby book.Title
                    where 
                        book.BookStatus == "Active" &&
                        (
                            book.Isbn.ToLower().Contains(input) ||
                            book.Title.ToLower().Contains(input) ||
                            book.Author.ToLower().Contains(input)
                        )
                    select book
                ).ToList();
            SetBooksSection();
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
        }
    }
}