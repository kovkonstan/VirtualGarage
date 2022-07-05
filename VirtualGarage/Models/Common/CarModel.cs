using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using AutoMapper;
using VirtualGarage.Logic.DataModel;
using VirtualGarage.Logic.Enums;

namespace VirtualGarage.Models
{
    public class CarModel : IDataErrorInfo
    {
        public Int32 CarID { get; set; }

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



        //---
        //public Int32 Page { get; set; }

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

    public class CarMapper
    {
        public CarMapper()
        {
            Mapper.CreateMap<VirtualGarage.Logic.DataModel.Car, CarModel>()
                .ForMember(dest => dest.AdditionalFeatures, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.BoxTypeName, opt => opt.MapFrom(src => (src.GearBoxType != null ? src.GearBoxType.GearBoxName : "")))
                .ForMember(dest => dest.MarkID, opt => opt.MapFrom(src => src.CarModel.CarMark.CarMarkID))
                .ForMember(dest => dest.ModelID, opt => opt.MapFrom(src => src.CarModel.CarModelID))
                .ForMember(dest => dest.BoxTypeID, opt => opt.MapFrom(src => src.GearBoxID))                
                .ForMember(dest => dest.CarcaseTypeName, opt => opt.MapFrom(src => (src.CarcaseType != null ? src.CarcaseType.CarcaseTypeName : "")))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => (src.Color != null ? src.Color.ColorName : "")))
                .ForMember(dest => dest.CurrencyName, opt => opt.MapFrom(src => src.Currency.CurrencyName))
                .ForMember(dest => dest.EngineVolume, opt => opt.MapFrom(src => (src.EngineVolume == null || src.EngineVolume == 0 ? "" : src.EngineVolume.ToString())))
                .ForMember(dest => dest.FuelTypeName, opt => opt.MapFrom(src => (src.FuelType != null ? src.FuelType.FuelTypeName : "")))
                .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.CarModel.CarModelName))
                .ForMember(dest => dest.MarkName, opt => opt.MapFrom(src => src.CarModel.CarMark.CarMarkName))
                .ForMember(dest => dest.Mileage, opt => opt.MapFrom(src =>(src.Mileage == null || src.Mileage == 0 ? "" : src.Mileage.ToString())))
                .ForMember(dest => dest.MonthMileage, opt => opt.MapFrom(src => (src.MonthMileage == null || src.MonthMileage == 0 ? "" : src.MonthMileage.ToString())))
                .ForMember(dest => dest.ReadOnly, opt => opt.MapFrom(src => src.CarIsReadOnly))
                .ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.CarVisible))
                .ForMember(dest => dest.CarPhoto, opt => opt.UseValue(null));                
        }

        public CarModel GetCarModel(VirtualGarage.Logic.DataModel.Car car)
        {
            return Mapper.Map<VirtualGarage.Logic.DataModel.Car, CarModel>(car);
        }
    }
}