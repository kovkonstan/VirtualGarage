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
    
    public partial class UserRole
    {
        public UserRole()
        {
            this.Users = new HashSet<User>();
        }
    
        public int UserRoleID { get; set; }
        public string UserRoleName { get; set; }
    
        public virtual ICollection<User> Users { get; set; }
    }
}
