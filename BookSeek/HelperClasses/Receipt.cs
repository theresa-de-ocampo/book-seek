using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSeek.HelperClasses
{
    public class Receipt
    {
        #pragma warning disable 0169
        int saleId;
        DateTime transactionDateTime;
        List<BookOnDataGrid> booksSold;
        decimal budget;
        decimal totalAmount;
        decimal additionalRequired;
        decimal cash;
        decimal change;

        public int SaleId { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public List<BookOnDataGrid> BooksSold { get; set; }
        public decimal Budget { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AdditionalRequired { get; set; }
        public decimal Cash { get; set; }
        public decimal Change { get; set; }
    }
}
