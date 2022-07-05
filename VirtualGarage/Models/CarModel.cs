using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using VirtualGarage.Enums;

namespace VirtualGarage.Models
{
    public class CarModel : GarageModel, IDataErrorInfo
    {
        [Required]
        [DisplayName("Марка")]
        public Int32 MarkID { get; set; }

        [DisplayName("Марка")]
        public String MarkName { get; set; }

        [Required]
        [DisplayName("Модель")]
        public Int32 ModelID { get; set; }        

        [DisplayName("Модель")]
        public String ModelName { get; set; }

        [DisplayName("Цвет")]
        public Int32? ColorID { get; set; }

        [DisplayName("Цвет")]
        public String ColorName { get; set; }

        [Required]
        [DisplayName("Тип топлива")]
        public Int32 FuelTypeID { get; set; }

        [DisplayName("Тип топлива")]
        public String FuelTypeName { get; set; }

        [DisplayName("Тип кузова")]
        public Int32? CarcaseTypeID { get; set; }

        [DisplayName("Тип кузова")]
        public String CarcaseTypeName { get; set; }

        [DisplayName("Тип коробки передач")]
        public Int32? BoxTypeID { get; set; }

        [DisplayName("Тип коробки передач")]
        public String BoxTypeName { get; set; }
        
        [MaxLength(5, ErrorMessage = "Введите число менее 100 000")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Введите число")]
        [DisplayName("Объем двигателя")]
        public String EngineVolume { get; set; }
                       
        [Required]
        [DisplayName("Дата покупки")]
        public DateTime BuyDate { get; set; }

        [Required]
        [DisplayName("Валюта")]
        public Int32 CurrencyID { get; set; }

        [DisplayName("Валюта")]
        public String CurrencyName { get; set; }

        [MaxLength(6, ErrorMessage = "Введите число менее 1 000 000")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Введите число")]
        [DisplayName("Текущий пробег")]
        public String Mileage { get; set; }

        [MaxLength(6, ErrorMessage = "Введите число менее 1 000 000")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Введите число")]
        [DisplayName("Средний пробег в месяц")]
        public String MonthMileage { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Дополнительные данные")]
        public String AdditionalFeatures { get; set; }

        public Boolean Visible { get; set; }

        public Boolean ReadOnly { get; set; }

        [Required]
        [DisplayName("Год выпуска")]
        public Int32 Year { get; set; }

        public Byte[] CarPhoto { get; set; }

        public String ImageType { get; set; }

        public Int32 Page { get; set; }

        public UserAccesOnCar UserAcces { get; set; }

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
                if (columnName == "BuyDate" &&
                    BuyDate == null)
                    return "Введите корректную дату";

                if (columnName == "ModelID" &&
                    ModelID == 0)
                    return "Выберите модель";

                return null; 
            }
        }        
    }
}