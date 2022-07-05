using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtualGarage.Logic.DataModel;

namespace VirtualGarage.Helpers
{
    public class StatisticHelper
    {
        public StatisticHelper(ICollection<Event> events)
        {
            _events = events; 
        }

        //public Int32 GetAvrMonthMileage()
        //{
        //    var events = _events.Where(it => it.Mileage != null && it.Mileage != 0)
        //                .OrderBy(it => it.Date).ToList();



        //    Int32 countOfDays = Convert.ToInt32((_events.Last().Date - _events.First().Date).TotalDays);
        //    Single avgMileageOnMonth = Convert.ToInt32((((Int32)_events.Last().Mileage - (Int32)_events.First().Mileage) / countOfDays) * 30);
        //}

        public ICollection<Event> Events
        {
            get { return _events; }
            set { _events = value; }
        }

        private ICollection<Event> _events;

    }
}