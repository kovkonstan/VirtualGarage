using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Models
{
    public class CarsInGarageModel 
    {
		public BaseGarageModel BaseModel { get; set; }

        public List<CarModel> Cars { get; set; }

        public Int32 CurrentPage { get; set; }

        public Int32 TotalPages { get; set; }

        public Boolean IsMyGarage { get; set; }

        public void Fill(String userName)
        {
            //LoginUserModel.Fill(userName);
        }
    }
}