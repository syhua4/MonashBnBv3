//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MonashBnBv3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Rating
    {
        public int ratingId { get; set; }
        public decimal ratingNum { get; set; }
        public string userId { get; set; }
        public int hotelId { get; set; }
        public int reserveId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Hotel Hotel { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
