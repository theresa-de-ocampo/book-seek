//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookSeek
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sale_Book
    {
        public int SaleId { get; set; }
        public string Isbn { get; set; }
        public int BookQuantitySold { get; set; }
        public decimal Price { get; set; }
    
        public virtual Book Book { get; set; }
        public virtual Sale Sale { get; set; }
    }
}
