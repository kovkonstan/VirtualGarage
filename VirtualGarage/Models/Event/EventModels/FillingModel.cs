using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace VirtualGarage.Models
{
    public class FillingModel : EventModel, IDataErrorInfo
    {
        [Required]
        [DisplayName("Тип топлива")]
        public Int32 FuelTypeID { get; set; }

        [DisplayName("Тип топлива")]
        public String FuelTypeName { get; set; }

        [DisplayName("Количество топлива")]
        public Single? FuelCount { get; set; }

        [DisplayName("Цена за единицу")]
		public Decimal? UnitCost { get; set; }

        public Boolean IsFullTank { get; set; }

        public Boolean IsForgotPreviousFilling { get; set; }

		string IDataErrorInfo.Error
		{
			get { return null; }
		}

		public override string this[string columnName]
		{
			get
			{
				if (columnName == "FuelCount" &&
					(FuelCount > 1000000 ||
					 FuelCount <= 0))
					return "Введите значение от 1 до 1 000 000";
				if (columnName == "UnitCost" &&
					(UnitCost > 1000000 ||
					 UnitCost <= 0))
					return "Введите значение от 1 до 1 000 000";

				return base[columnName];
			}
		}
	}
}