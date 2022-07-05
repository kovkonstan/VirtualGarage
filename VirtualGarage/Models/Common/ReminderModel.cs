using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace VirtualGarage.Models
{
    public class ReminderModel 
    {
		public BaseDefaultModel BaseModel { get; set; }

        public Int32 ReminderID { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage="Длина имени должна быть не более 100 символов")]
        [MinLength(5, ErrorMessage="Длина имени должна быть не более 5 символов")]
        [DisplayName("Имя события")]
        public String Title { get; set; }

        [Required]
        [DisplayName("Дата и время начала")]
        public DateTime? StartDateTime { get; set; }

        [Required]
        [DisplayName("Дата и время окончания")]
        public DateTime? FinishDateTime { get; set; }

        //[Required]
        //[DisplayName("Прислать напоминания на E-mail")]
        //public Boolean IsRemindByEmail { get; set; }

        //[DisplayName("Напомнить за")]
        //public Int32? ReminderCountOfDays { get; set; }

        [Required]
        [DisplayName("Выполнено")]
        public Boolean IsReminderDone { get; set; }

        [Required]
        [DisplayName("Автомобиль")]
        public Int32 CarID { get; set; }

		public Boolean IsNeedRemind { get; set; }
         
    }

    public class ReminderMapper
    {
        public ReminderMapper()
        {
            Mapper.CreateMap<VirtualGarage.Logic.DataModel.Reminder, ReminderModel>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ReminderName));
            Mapper.CreateMap<ReminderModel, VirtualGarage.Logic.DataModel.Reminder>()
                .ForMember(dest => dest.ReminderName, opt => opt.MapFrom(src => src.Title));
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

        public ReminderModel GetReminderModel(VirtualGarage.Logic.DataModel.Reminder rem)
        {
            return Mapper.Map<VirtualGarage.Logic.DataModel.Reminder, ReminderModel>(rem);
        }

        public VirtualGarage.Logic.DataModel.Reminder GetReminder(ReminderModel remModel)
        {
            return Mapper.Map<ReminderModel, VirtualGarage.Logic.DataModel.Reminder>(remModel);
        }
    }
}