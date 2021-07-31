using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookSeek.HelperClasses
{
    public class Track
    {
        int trackId;
        string trackTag;
        string isbn;
        string title;
        int quantity;
        DateTime eventDateTime;

        public int TrackId { get; set; }
        public string TrackTag { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public DateTime EventDateTime { get; set; }

        public static void Add
            (string trackTag, string isbn, string title, int quantity, DateTime eventDateTime, Database database)
        {
            Ledger ledger = new Ledger()
            {
                TrackTag = trackTag,
                Isbn = isbn,
                Quantity = quantity,
                EventDateTime = eventDateTime
            };

            database.Ledger.Add(ledger);
            database.SaveChanges();
        }
    }
}
