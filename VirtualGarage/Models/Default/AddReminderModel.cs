﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;

namespace VirtualGarage.Models
{
    public class AddReminderModel : ReminderModel
    {
        public List<SelectListItem> AllUserCars { get; set; }

		//public override void Fill(string userName)
		//{
		//    base.Fill(userName);

		//    using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
		//    {
		//        var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

		//        this.AllUserCars = (from car in userRepo.GetUserCars(userName)
		//                                select new 
		//                                {
		//                                    CarID = car.CarID,
		//                                    Mark = car.CarModel.CarMark.CarMarkName,
		//                                    Model = car.CarModel.CarModelName,
		//                                    Year = car.Year
		//                                }).ToList()
		//                                .Select(it => new SelectListItem()
		//                                {
		//                                    Text = it.Mark + " " + it.Model + " " + it.Year.ToString(),
		//                                    Value = it.CarID.ToString()

		//                                }).ToList();               
		//    }
		//}
    }
}