using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Models
{
	public class EventInfoModel
	{
        public BaseCarModel BaseModel { get; set; }

		public EventInfoModel()	: base()
		{			 
		}

		public EventInfoModel(Int32 eventID)
			: base()
		{
			EventID = eventID;
		}

		public Int32 EventID { get; set; }
	}
}