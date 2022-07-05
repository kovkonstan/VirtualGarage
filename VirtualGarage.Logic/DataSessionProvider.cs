using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TmpLabs.DataEngine.EF;
using System.Data.Entity;

namespace VirtualGarage.Logic
{
    internal class DataSessionProvider : EntityFrameworkSessionProvider<VirtualGarageDB>
    {
        ///// <summary>
        ///// Sets the initializer.
        ///// </summary>
        //protected override void SetInitializer()
        //{
        //    if (TmpLabs.DataEngine.Configuration.UseDataUpdate)
        //    {
        //        Database.SetInitializer<VirtualGarageDB>(new DemoDBInitializer());
        //    }
        //    else
        //    {
        //        Database.SetInitializer<VirtualGarageDB>(null);
        //    }
        //}
    }
}
