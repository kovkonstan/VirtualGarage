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
    
    public partial class Color
    {
        public Color()
        {
            this.Cars = new HashSet<Car>();
        }
    
        public int ColorID { get; set; }
        public string ColorName { get; set; }
    
        public virtual ICollection<Car> Cars { get; set; }
    }
}
