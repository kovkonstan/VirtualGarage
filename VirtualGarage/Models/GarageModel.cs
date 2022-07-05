using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Enums;

namespace VirtualGarage.Models
{
    public class GarageModel : LoginUserModel
    {
        public Int32 CarID { get; set; }

        public UserAccesOnCar UserAcces { get; set; }

        public String PageName { get; set; } 

        public List<SimilarCarModel> SimilarCars { get; set; }

        public override void Fill(String userName)
        {
            base.Fill(userName);

            this.SimilarCars = new List<SimilarCarModel>()
            {
                new SimilarCarModel()
                {
                    CarID = 69,
                    CarName = "Mercedes S500 1991",
                    UserID = 14,
                    UserName = "qwe"
                },
                new SimilarCarModel()
                {
                    CarID = 69,
                    CarName = "Mercedes S500 1991",
                    UserID = 14,
                    UserName = "qwe"
                },
                new SimilarCarModel()
                {
                    CarID = 69,
                    CarName = "Mercedes S500 1991",
                    UserID = 14,
                    UserName = "qwe"
                }
            };
        }
    }
}