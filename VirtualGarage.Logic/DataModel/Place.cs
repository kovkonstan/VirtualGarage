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
    
    public partial class Place
    {
        public Place()
        {
            this.Events = new HashSet<Event>();
        }
    
        public int PlaceID { get; set; }
        public string PlaceName { get; set; }
        public bool IsFillingStation { get; set; }
        public Nullable<int> UserID { get; set; }
    
        public virtual ICollection<Event> Events { get; set; }
        public virtual User User { get; set; }
    }
}
