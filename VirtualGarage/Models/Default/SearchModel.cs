using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Logic.DataModel;
using System.ComponentModel;

namespace VirtualGarage.Models
{
    public class SearchModel 
    {
		public BaseDefaultModel BaseModel { get; set; }

		[DisplayName("Марка")]
		public Int32? MarkID { get; set; }
				
		[DisplayName("Модель")]
		public Int32? ModelID { get; set; }

		[DisplayName("Пробег")]
		public Int32? LowMileage { get; set; }

		public Int32? HighMileage { get; set; }

		[DisplayName("Год выпуска")]
		public Int32? LowYear { get; set; }
				
		public Int32? HighYear { get; set; }

		public List<CarModel> Cars { get; set; }

		public Int32 CurrentPage { get; set; }

		public Int32 TotalPages { get; set; }

		public List<SelectListItem> AllMarks { get; set; }

		public List<SelectListItem> AllModels { get; set; }

		public IEnumerable<SelectListItem> GetAllYears()
		{
			List<SelectListItem> result = new List<SelectListItem>();
			for (int i = DateTime.Now.Year; i >= 1930; i--)
			{
				result.Add(new SelectListItem()
				{
					Text = i.ToString(),
					Value = i.ToString()
				});
			}

			return result;
		}

		//public override void Fill(String userName)
		//{
		//    base.Fill(userName);

		//    using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
		//    {
		//        var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

		//        this.AllMarks = unitOfWork.CreateRepo<CarMark>().Select(mark => new
		//        {
		//            Text = mark.CarMarkName,
		//            Value = mark.CarMarkID
		//        }).ToList().Select(t => new SelectListItem()
		//        {
		//            Text = t.Text,
		//            Value = t.Value.ToString()
		//        }).ToList();

		//        this.AllModels = new List<SelectListItem>();
		//    }
		//}
	}
}