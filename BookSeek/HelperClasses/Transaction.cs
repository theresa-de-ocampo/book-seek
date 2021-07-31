using System;

namespace BookSeek.HelperClasses
{
    public class Transaction
    {
        #pragma warning disable 0169
        private int saleId;
        private DateTime transactionDateTime;
        private string purchases;
        private bool validForReturn;
        private string toolTipMessage;

        public int SaleId { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string Purchases { get; set; }
        public bool ValidForReturn { get; set; }
        public string ToolTipMessage { get; set; }
    }
}
