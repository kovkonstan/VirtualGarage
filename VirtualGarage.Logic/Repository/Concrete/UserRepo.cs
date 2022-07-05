using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InostudioSolutions.Data;
using VirtualGarage.Logic.DataModel;
using VirtualGarage.Logic.Exceptions;
using VirtualGarage.Logic.Enums;

namespace VirtualGarage.Logic.Repository
{
    class UserRepo : RepositoryEF<User>, IUserRepo
    {
        public UserRepo(IUnitOfWorkEF unitOfWork)
            : base(unitOfWork)
        {
        }

        public User GetByLogin(string userName)
        {
            return this.SingleOrDefault(it => it.UserNick == userName);
        }

        public Boolean IsUserNameExist(String userName)
        {
            return this.Any(it => it.UserNick == userName);
        }

        public bool IsEmailExist(string userEmail)
        {
            return this.Any(it => it.UserEmail == userEmail);
        }

        public IEnumerable<Car> GetUserCarsByPage(int currentPage, int pageSize, String userName)
        {
            return this.GetUserCars(userName).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public IEnumerable<Car> GetUserCarsByPage(int currentPage, int pageSize, int userID)
        {
            return this.GetUserCars(userID).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }
        
        public IEnumerable<Car> GetUserCars(String userName)
        {
            var user = this.SingleOrDefault(it => it.UserNick == userName);
            if (user != null)
            {
				return user.CarInGarages.Select(it => it.Car);
            }
            else
            {
                throw new UserNotExistException();
            }
        }

        public IEnumerable<Car> GetUserCars(int userID)
        {
            var user = this.SingleOrDefault(it => it.UserID == userID);
            if (user != null)
            {
                return user.CarInGarages.Select(it => it.Car);
            }
            else
            {
                throw new UserNotExistException();
            }
        }
        
        //public UserAccesOnCar CheckUserAcces(int carID, String userName)
        //{
        //    //var car = this.SingleOrDefault(it => it.CarID == carID);
        //    //if (car != null)
        //    //{
        //    //    var carInGarage = car.CarInGarages.FirstOrDefault(it => it.User.UserNick == userName);

        //    //    if (carInGarage != null && !car.CarIsReadOnly)
        //    //    {
        //    //        return carInGarage.IsOwner ? UserAccesOnCar.Own : UserAccesOnCar.Manage;
        //    //    }
        //    //    else if (car.CarVisible)
        //    //    {
        //    //        return UserAccesOnCar.Read;
        //    //    }
        //    //}
        //    //return UserAccesOnCar.Close;  



        //    var user = this.GetByLogin(userName);
        //    var carInGarage = user.CarInGarages.FirstOrDefault(it => it.User.UserNick == userName);
            

        //    if (carInGarage != null && !car.CarIsReadOnly)
        //    {
        //        return carInGarage.IsOwner ? UserAccesOnCar.Own : UserAccesOnCar.Manage;
        //    }
        //    else if (car.CarVisible)
        //    {
        //        return UserAccesOnCar.Read;
        //    }

        //    //if (user.CarInGarages.Any(it => it.CarID == carID))
        //    //{
        //    //    if (user.CarInGarages.Single(it => it.CarID == carID).IsOwner)
        //    //    {
        //    //        return UserAccesOnCar.Own;
        //    //    }
        //    //    else
        //    //    {
        //    //        return UserAccesOnCar.Manage;
        //    //    }
        //    //}
        //    //else return UserAccesOnCar.Close;
        //}

        public LoginResult CheckLoginAndPass(string login, string pass)
        {
            User user;
            if ((user = this.GetByLogin(login)) == null)
            {
                return LoginResult.UserNotExist;
            }
            else if (user.UserPassword != pass)
            {
                return LoginResult.UncorrectPass;
            }
            else
            {
                return LoginResult.Success;
            }
        }

        public int GetCountOfUserCars(string userName)
        {
            return this.GetUserCars(userName).Count();
        }

        public int GetCountOfUserCars(int userID)
        {
            return this.GetUserCars(userID).ToList().Count;
        }

        public IEnumerable<Car> GetUserCars(string userName, bool isOwnerRequest)
        {
            if (isOwnerRequest) // Если пользователь запрашивает автомобили из своего гаража
            {
                return this.GetUserCars(userName);
            }
            else
            {
                return this.GetUserCars(userName).Where(it => it.CarVisible);
            }            
        }

        public IEnumerable<Car> GetUserCars(int userID, bool isOwnerRequest)
        {
            if (isOwnerRequest) // Если пользователь запрашивает автомобили из своего гаража
            {
                return this.GetUserCars(userID);
            }
            else
            {
                return this.GetUserCars(userID).Where(it => it.CarVisible);
            }      
        }


        public IEnumerable<Car> GetUserCarsByPage(int currentPage, int pageSize, int userID, bool isOwnerRequest)
        {
            return this.GetUserCars(userID, isOwnerRequest).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        public IEnumerable<Car> GetUserCarsByPage(int currentPage, int pageSize, string userName, bool isOwnerRequest)
        {
            return this.GetUserCars(userName, isOwnerRequest).Skip((currentPage - 1) * pageSize).Take(pageSize);
        }
    }
}
