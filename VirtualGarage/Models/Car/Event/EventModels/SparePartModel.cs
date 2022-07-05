using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using VirtualGarage.Logic;
using VirtualGarage.Logic.DataModel;

namespace VirtualGarage.Models
{
    public class SparePartModel	 : IDataErrorInfo
    {
        public SparePartModel()
        {
            this.Fill();
        }

		public SparePartModel(Int32 numberInPage)
		{
			this.NumberInPage = numberInPage;
			this.Fill();
		}

		[DisplayName("Тип запчасти")]
        public Int32? SparePartTypeID { get; set; }

        [DisplayName("Тип запчасти")]
        public String SparePartTypeName { get; set; }

        [DisplayName("Производитель")]
        public String Producer { get; set; }

        [DisplayName("Модель")]
        public String Model { get; set; }

        [DisplayName("Цена")]
        public Decimal? SparePartCost { get; set; }

        [DisplayName("Цена за работу(установку)")]
		public Decimal? WorkCost { get; set; }
					 
		[Required]
        public Boolean IsRepair { get; set; }

		public Int32 NumberInPage { get; set; }

        public IEnumerable<SelectListItem> AllSparePartTypes { get; set; }

        public void Fill()
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                this.AllSparePartTypes = unitOfWork.CreateRepo<SparePartType>().Select(sptype => new
                {
                    Text = sptype.SparePartTypeName,
                    Value = sptype.SparePartTypeID
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();
            }
        }

		public string Error
		{
			get
			{
				return null;
			}
		}

		public string this[string columnName]
		{
			get
			{
				if (columnName == "SparePartCost" &&
					(SparePartCost > 1000000 ||
					 SparePartCost <= 0))
					return "Введите значение от 1 до 1 000 000";
				if (columnName == "WorkCost" &&
					(WorkCost > 1000000 ||
					 WorkCost <= 0))
					return "Введите значение от 1 до 1 000 000";

				return null;
			}
		}
	}
}