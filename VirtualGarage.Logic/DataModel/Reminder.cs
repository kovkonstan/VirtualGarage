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
    
    public partial class Reminder
    {
        public int ReminderID { get; set; }
        public int UserID { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public System.DateTime FinishDateTime { get; set; }
        public string ReminderName { get; set; }
        public bool IsRemindByEmail { get; set; }
        public Nullable<int> ReminderCountOfMileage { get; set; }
        public Nullable<int> ReminderCountOfDays { get; set; }
        public bool IsReminderDone { get; set; }
        public int CarID { get; set; }
    
        public virtual Car Car { get; set; }
        public virtual User User { get; set; }
    }
}
