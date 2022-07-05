using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace VirtualGarage.Models
{
    public class ReminderInfoModel : ReminderModel
    {
        public String CarName { get; set; }
    }

    public class ReminderInfoMapper
    {
        public ReminderInfoMapper()
        {
            Mapper.CreateMap<VirtualGarage.Logic.DataModel.Reminder, ReminderInfoModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ReminderName));
            Mapper.CreateMap<ReminderInfoModel, VirtualGarage.Logic.DataModel.Reminder>()
                .ForMember(dest => dest.ReminderName, opt => opt.MapFrom(src => src.Title));
        }

        public ReminderInfoModel GetReminderInfoModel(VirtualGarage.Logic.DataModel.Reminder rem)
        {
            return Mapper.Map<VirtualGarage.Logic.DataModel.Reminder, ReminderInfoModel>(rem);
        }

        public VirtualGarage.Logic.DataModel.Reminder GetReminder(ReminderInfoModel remModel)
        {
            return Mapper.Map<ReminderInfoModel, VirtualGarage.Logic.DataModel.Reminder>(remModel);
        }
    }
}