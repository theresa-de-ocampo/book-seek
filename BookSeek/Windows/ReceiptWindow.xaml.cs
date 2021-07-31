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
using System.Windows.Shapes;
using BookSeek.HelperClasses;

namespace BookSeek.Windows
{
    /// <summary>
    /// Interaction logic for ReceiptWindow.xaml
    /// </summary>
    public partial class ReceiptWindow : Window
    {
        Database database = new Database();
        public ReceiptWindow(Receipt receipt)
        {
            InitializeComponent();
            OrderNumberLabel.Content = receipt.SaleId.ToString();
            TransactionDate.Content = receipt.TransactionDateTime.ToString();
            TransactionDataGrid.ItemsSource = receipt.BooksSold;
            TotalAmountLabel.Content = receipt.TotalAmount.ToString();
            CashLabel.Content = receipt.Cash.ToString();
            ChangeLabel.Content = receipt.Change.ToString();

            int n = 0;
            foreach (var bookSold in receipt.BooksSold)
            {
                n += bookSold.Quantity;
            }
            TotalNumberOfBooksLabel.Content = n.ToString();
        }
    }
}
