using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Logic.DataModel;

namespace VirtualGarage.Models
{
    public class AddCarModel : CarModel
    {
        public IEnumerable<SelectListItem> AllMarks { get; set; }

        public IEnumerable<SelectListItem> AllModels { get; set; }

        public IEnumerable<SelectListItem> AllColors { get; set; }

        public IEnumerable<SelectListItem> AllFuelTypes { get; set; }

        public IEnumerable<SelectListItem> AllCarcaseTypes { get; set; }

        public IEnumerable<SelectListItem> AllGearBoxTypes { get; set; }

        public IEnumerable<SelectListItem> AllCurrences { get; set; }

        public IEnumerable<SelectListItem> GetAllYears()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            for (int i = DateTime.Now.Year; i >= 1930; i--)
			{
                result.Add(new SelectListItem()
                {
                    Text = i.ToString(),
                    Value = i.ToString()                    
                });		 
			}

            return result;
        }

        public void Fill()
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

                this.AllColors = unitOfWork.CreateRepo<Color>().Select(color => new
                {
                    Text = color.ColorName,
                    Value = color.ColorID,
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();

                this.AllCurrences = unitOfWork.CreateRepo<Currency>().Select(cur => new
                {
                    Text = cur.CurrencyName,
                    Value = cur.CurrencyID,
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();

                this.AllFuelTypes = unitOfWork.CreateRepo<FuelType>().Select(ftype => new
                {
                    Text = ftype.FuelTypeName,
                    Value = ftype.FuelTypeID
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();

                this.AllGearBoxTypes = unitOfWork.CreateRepo<GearBoxType>().Select(gbtype => new
                {
                    Text = gbtype.GearBoxName,
                    Value = gbtype.GearBoxID
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();

                this.AllMarks = unitOfWork.CreateRepo<CarMark>().Select(mark => new
                {
                    Text = mark.CarMarkName,
                    Value = mark.CarMarkID
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();

                this.AllModels = new List<SelectListItem>();

                this.AllCarcaseTypes = unitOfWork.CreateRepo<CarcaseType>().Select(carctype => new
                {
                    Text = carctype.CarcaseTypeName,
                    Value = carctype.CarcaseTypeID
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();
            }
        }
    }
}