//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VirtualGarage.Logic.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Event
    {
        public Event()
        {
            this.Fillings = new HashSet<Filling>();
            this.Repairs = new HashSet<Repair>();
        }
    
        public int EventID { get; set; }
        public int EventTypeID { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<int> Mileage { get; set; }
        public Nullable<decimal> GeneralCost { get; set; }
        public string EventComments { get; set; }
        public int CurrencyID { get; set; }
        public int PlaceID { get; set; }
        public int CarID { get; set; }
        public int UserID { get; set; }
    
        public virtual Car Car { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual EventType EventType { get; set; }
        public virtual Place Place { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Filling> Fillings { get; set; }
        public virtual ICollection<Repair> Repairs { get; set; }
    }
}