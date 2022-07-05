using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;
using AutoMapper;

namespace VirtualGarage.Models
{
	public class EditReminderModel : ReminderModel
	{
		public List<SelectListItem> AllUserCars { get; set; }

		//public override void Fill(string userName)
		//{
		//    base.Fill(userName);

		//    using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
		//    {
		//        var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

		//        this.AllUserCars = (from car in userRepo.GetUserCars(userName)
		//                            select new
		//                            {
		//                                CarID = car.CarID,
		//                                Mark = car.CarModel.CarMark.CarMarkName,
		//                                Model = car.CarModel.CarModelName,
		//                                Year = car.Year
		//                            }).ToList()
		//                                .Select(it => new SelectListItem()
		//                                {
		//                                    Text = it.Mark + " " + it.Model + " " + it.Year.ToString(),
		//                                    Value = it.CarID.ToString()

		//                                }).ToList();
		//    }
		//}
	}

	public class EditReminderModelMapper
	{
		public EditReminderModelMapper()
		{
			Mapper.CreateMap<VirtualGarage.Logic.DataModel.Reminder, EditReminderModel>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ReminderName));
			Mapper.CreateMap<EditReminderModel, VirtualGarage.Logic.DataModel.Reminder>()
				.ForMember(dest => dest.ReminderName, opt => opt.MapFrom(src => src.Title));
		}

		public EditReminderModel GetEditReminderModel(VirtualGarage.Logic.DataModel.Reminder rem)
		{
			return Mapper.Map<VirtualGarage.Logic.DataModel.Reminder, EditReminderModel>(rem);
		}

		public VirtualGarage.Logic.DataModel.Reminder GetReminder(EditReminderModel remModel)
		{
			return Mapper.Map<EditReminderModel, VirtualGarage.Logic.DataModel.Reminder>(remModel);
		}
	}
}