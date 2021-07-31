using BookSeek.HelperClasses;
using System.Linq;
using System.Windows;

namespace BookSeek.Windows
{
    /// <summary>
    /// Interaction logic for ExchangeReceiptWindow.xaml
    /// </summary>
    public partial class ExchangeReceiptWindow : Window
    {
        Database database = new Database();
        public ExchangeReceiptWindow(Receipt receipt)
        {
            InitializeComponent();
            OrderNumberLabel.Content = receipt.SaleId.ToString();
            TransactionDate.Content = receipt.TransactionDateTime.ToString();
            TransactionDataGrid.ItemsSource = receipt.BooksSold;
            BudgetLabel.Content = receipt.Budget.ToString();
            TotalAmountLabel.Content = receipt.TotalAmount.ToString();
            AdditionalRequiredLabel.Content = receipt.AdditionalRequired.ToString();
            CashLabel.Content = receipt.Cash.ToString();
            ChangeLabel.Content = receipt.Change.ToString();
            TotalNumberOfBooksLabel.Content = receipt.BooksSold.Count().ToString();
        }
    }
}
