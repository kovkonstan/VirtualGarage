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
    
    public partial class User
    {
        public User()
        {
            this.CarInGarages = new HashSet<CarInGarage>();
            this.Events = new HashSet<Event>();
            this.EventTypes = new HashSet<EventType>();
            this.Messages = new HashSet<Message>();
            this.Messages1 = new HashSet<Message>();
            this.Places = new HashSet<Place>();
            this.Reminders = new HashSet<Reminder>();
        }
    
        public int UserID { get; set; }
        public string UserNick { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public int UserRoleID { get; set; }
        public Nullable<bool> IsCorporativeAccount { get; set; }
    
        public virtual ICollection<CarInGarage> CarInGarages { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<EventType> EventTypes { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Message> Messages1 { get; set; }
        public virtual ICollection<Place> Places { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }
        public virtual UserRole UserRole { get; set; }
    }
}