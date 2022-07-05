using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtualGarage.Logic;
using VirtualGarage.Logic.DataModel;
using VirtualGarage.Logic.Repository;
using System.Web.Mvc;

namespace VirtualGarage.Models
{
    public class AddEventDivModel : EventModel
    {
        public IEnumerable<SelectListItem> AllEventTypes { get; set; }

        public IEnumerable<SelectListItem> AllCurrences { get; set; }

        public IEnumerable<SelectListItem> AllEventPlaces { get; set; }

        public void Fill()
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {

                var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();

                //this.AllEventTypes = unitOfWork.CreateRepo<EventType>().Select(eventType => new
                //{
                //    Text = eventType.EventTypeName,
                //    Value = eventType.EventTypeID
                //}).ToList().Select(t => new SelectListItem()
                //{
                //    Text = t.Text,
                //    Value = t.Value.ToString()
                //}).ToList();

                this.AllCurrences = unitOfWork.CreateRepo<Currency>().Select(cur => new
                {
                    Text = cur.CurrencyName,
                    Value = cur.CurrencyID
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();

                this.AllEventPlaces = unitOfWork.CreateRepo<Place>().Select(place => new
                {
                    Text = place.PlaceName,
                    Value = place.PlaceID
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();


                //this.AllColors = unitOfWork.CreateRepo<Color>().Select(color => new
                //{
                //    Text = color.ColorName,
                //    Value = color.ColorID,
                //}).ToList().Select(t => new SelectListItem()
                //{
                //    Text = t.Text,
                //    Value = t.Value.ToString()
                //}).ToList();

                //this.AllCurrences = unitOfWork.CreateRepo<Currency>().Select(cur => new
                //{
                //    Text = cur.CurrencyName,
                //    Value = cur.CurrencyID,
                //}).ToList().Select(t => new SelectListItem()
                //{
                //    Text = t.Text,
                //    Value = t.Value.ToString()
                //}).ToList();

                //this.AllFuelTypes = unitOfWork.CreateRepo<FuelType>().Select(ftype => new
                //{
                //    Text = ftype.FuelTypeName,
                //    Value = ftype.FuelTypeID
                //}).ToList().Select(t => new SelectListItem()
                //{
                //    Text = t.Text,
                //    Value = t.Value.ToString()
                //}).ToList();

                //this.AllGearBoxTypes = unitOfWork.CreateRepo<GearBoxType>().Select(gbtype => new
                //{
                //    Text = gbtype.GearBoxName,
                //    Value = gbtype.GearBoxID
                //}).ToList().Select(t => new SelectListItem()
                //{
                //    Text = t.Text,
                //    Value = t.Value.ToString()
                //}).ToList();

                //this.AllMarks = unitOfWork.CreateRepo<CarMark>().Select(mark => new
                //{
                //    Text = mark.CarMarkName,
                //    Value = mark.CarMarkID
                //}).ToList().Select(t => new SelectListItem()
                //{
                //    Text = t.Text,
                //    Value = t.Value.ToString()
                //}).ToList();

                //this.AllModels = new List<SelectListItem>();

                //this.AllCarcaseTypes = unitOfWork.CreateRepo<CarcaseType>().Select(carctype => new
                //{
                //    Text = carctype.CarcaseTypeName,
                //    Value = carctype.CarcaseTypeID
                //}).ToList().Select(t => new SelectListItem()
                //{
                //    Text = t.Text,
                //    Value = t.Value.ToString()
                //}).ToList();
            }
        }
    }
}