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
    
    public partial class Currency
    {
        public Currency()
        {
            this.Cars = new HashSet<Car>();
            this.Events = new HashSet<Event>();
        }
    
        public int CurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public string CultureName { get; set; }
    
        public virtual ICollection<Car> Cars { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
