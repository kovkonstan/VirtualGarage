//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VirtualGarage.Logic.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Filling
    {
        public int FillingID { get; set; }
        public int FuelTypeID { get; set; }
        public Nullable<float> FuelCount { get; set; }
        public Nullable<decimal> UnitCost { get; set; }
        public bool IsFullTank { get; set; }
        public bool IsForgotPreviousFilling { get; set; }
        public int EventID { get; set; }
    
        public virtual Event Event { get; set; }
        public virtual FuelType FuelType { get; set; }
    }
}
