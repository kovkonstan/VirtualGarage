using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VirtualGarage.Models
{
    public class BaseEventModel : IDataErrorInfo
    {
        public Int32 EventID { get; set; }

        [Required]
        [DisplayName("Тип события")]
        public Int32 EventTypeID { get; set; }

        [Required]
        [DisplayName("Тип события")]
        public String EventTypeName { get; set; }

        [Required]
        [DisplayName("Дата события")]
        public DateTime Date { get; set; }

        [MaxLength(6, ErrorMessage = "Введите число менее 1 000 000")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Введите число")]
        [DisplayName("Текущий пробег")]
        public Int32 Mileage { get; set; }

        [MaxLength(6, ErrorMessage = "Введите число менее 1 000 000")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Введите число")]
        [DisplayName("Общая стоимость")]
        public Int32 GeneralCost { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Комментарий")]
        public String EventComments { get; set; }

        [Required]
        [DisplayName("Валюта")]
        public Int32 CurrencyID { get; set; }

        [DisplayName("Валюта")]
        public String CurrencyName { get; set; }

        [Required]
        [DisplayName("Место проведения")]
        public Int32 PlaceID { get; set; }

        [Required]
        [DisplayName("Место проведения")]
        public String PlaceName { get; set; }

        public Int32 CarID { get; set; }

        public Int32 UserID { get; set; }


        public string Error
        {
            get { return null; }
        }

        public virtual string this[string columnName]
        {
            get
            {
                if (columnName == "BuyDate" &&
                    Date == null)
                    return "Введите корректную дату";

                return null;
            }
        }
    }
}