using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using AutoMapper;
using VirtualGarage.Logic.DataModel;

namespace VirtualGarage.Models
{
    public class EventModel : IDataErrorInfo
    {
        public Int32 EventID { get; set; }

        [Required]
        [DisplayName("Тип события")]
        public Int32 EventTypeID { get; set; }

        [DisplayName("Тип события")]
        public String EventTypeName { get; set; }

        [Required]
        [DisplayName("Дата события")]
        public DateTime Date { get; set; }
		        
        [DisplayName("Текущий пробег")]
        public Int32? Mileage { get; set; }
                
        [DisplayName("Общая стоимость")]
        public Decimal? GeneralCost { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Комментарий")]
        public String EventComments { get; set; }

        [Required]
        [DisplayName("Валюта")]
        public Int32 CurrencyID { get; set; }

        [DisplayName("Валюта")]
        public String CurrencyName { get; set; }

		public String CurrencyAbbreviation { get; set; }

        [DisplayName("Место проведения")]
        public Int32 PlaceID { get; set; }

        [DisplayName("Место проведения")]
        public String PlaceName { get; set; }

        [DisplayName("По дням через")]
        public Boolean IsAddReminderByDays { get; set; }

		[DisplayName("По пробегу через")]
		public Boolean IsAddReminderByMileage { get; set; }

        public Int32? CountOfDaysForRemind { get; set; }

        public Int32? CountOfMileageForRemind { get; set; }

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
                if (columnName == "Date" &&
                    Date == null)
                    return "Введите корректную дату";
				if (columnName == "Mileage" &&
					(Mileage > 1000000 ||
					 Mileage <= 0))
					return "Введите значение от 1 до 1 000 000";
				if (columnName == "GeneralCost" &&
					(GeneralCost > 1000000 ||
					 GeneralCost <= 0))
					return "Введите значение от 1 до 1 000 000";

                return null;
            }
        }       
    }

    public class EventMapper
    {
        public EventMapper()
        {
            Mapper.CreateMap<Event, EventModel>();
                //.ForMember(dest => dest.EventTypeName, opt => opt.MapFrom(src => src.EventType.EventTypeName))
                //.ForMember(dest => dest.PlaceName, opt => opt.MapFrom(src => src.Place.PlaceName))
                //.ForMember(dest => dest.CurrencyName, opt => opt.MapFrom(src => src.Currency.CurrencyName));

                //.ForMember(dest => dest.AdditionalFeatures, opt => opt.MapFrom(src => src.Comment))
                //.ForMember(dest => dest.BoxTypeName, opt => opt.MapFrom(src => (src.GearBoxType != null ? src.GearBoxType.GearBoxName : "")))
                //.ForMember(dest => dest.MarkID, opt => opt.MapFrom(src => src.CarModel.CarMark.CarMarkID))
                //.ForMember(dest => dest.ModelID, opt => opt.MapFrom(src => src.CarModel.CarModelID))
                //.ForMember(dest => dest.BoxTypeID, opt => opt.MapFrom(src => src.GearBoxID))
                //.ForMember(dest => dest.CarcaseTypeName, opt => opt.MapFrom(src => (src.CarcaseType != null ? src.CarcaseType.CarcaseTypeName : "")))
                //.ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => (src.Color != null ? src.Color.ColorName : "")))
                //.ForMember(dest => dest.CurrencyName, opt => opt.MapFrom(src => src.Currency.CurrencyName))
                //.ForMember(dest => dest.EngineVolume, opt => opt.MapFrom(src => (src.EngineVolume == null || src.EngineVolume == 0 ? "" : src.EngineVolume.ToString())))
                //.ForMember(dest => dest.FuelTypeName, opt => opt.MapFrom(src => (src.FuelType != null ? src.FuelType.FuelTypeName : "")))
                //.ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.CarModel.CarModelName))
                //.ForMember(dest => dest.MarkName, opt => opt.MapFrom(src => src.CarModel.CarMark.CarMarkName))
                //.ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => (src.Mileage == null || src.Mileage == 0 ? "" : src.Mileage.ToString())))
                //.ForMember(dest => dest.MonthMileage, opt => opt.MapFrom(src => (src.MonthMileage == null || src.MonthMileage == 0 ? "" : src.MonthMileage.ToString())))
                //.ForMember(dest => dest.ReadOnly, opt => opt.MapFrom(src => src.CarIsReadOnly))
                //.ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.CarVisible))
                //.ForMember(dest => dest.CarPhoto, opt => opt.UseValue(null));
        }

        public EventModel GetEventModel(Event ev)
        {
            return Mapper.Map<Event, EventModel>(ev);
        }
    }
}