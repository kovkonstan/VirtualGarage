using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InostudioSolutions.Data;
using VirtualGarage.Logic.DataModel;
using VirtualGarage.Logic.Enums;

namespace VirtualGarage.Logic.Repository
{
    public interface IUserRepo : IRepository<User>
    {
        /// <summary>
        /// get user by login or return null, if it no
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        User GetByLogin(String userName);

        /// <summary>
        /// Проверяет данные пользователя при входе
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        LoginResult CheckLoginAndPass(String login, String pass);

        /// <summary>
        /// Проверяет, существует ли пользователь с указанным именем
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Boolean IsUserNameExist(String userName);

        /// <summary>
        /// Проверяет, существует ли пользователь с указанным e-mail'ом
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        Boolean IsEmailExist(String userName);

        /// <summary>
        /// Получить автомобили, которые необходимо отобразить на запрашиваемой странице
        /// при указанном количестве автомобилей на странице
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        IEnumerable<Car> GetUserCarsByPage(Int32 currentPage, Int32 pageSize, String userName);

        /// <summary>
        /// Получить автомобили пользователя, которые необходимо отобразить на запрашиваемой странице
        /// при указанном количестве автомобилей на странице
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="userID"></param>
        /// <param name="isOwnerRequest"></param>
        /// <returns></returns>
        IEnumerable<Car> GetUserCarsByPage(Int32 currentPage, Int32 pageSize, Int32 userID, Boolean isOwnerRequest);

        /// <summary>
        /// Получить автомобили, которые необходимо отобразить на запрашиваемой странице
        /// при указанном количестве автомобилей на странице
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="userName"></param>
        /// <param name="isOwnerRequest"></param>
        /// <returns></returns>
        IEnumerable<Car> GetUserCarsByPage(Int32 currentPage, Int32 pageSize, String userName, Boolean isOwnerRequest);

        /// <summary>
        /// Получить автомобили пользователя, которые необходимо отобразить на запрашиваемой странице
        /// при указанном количестве автомобилей на странице
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        IEnumerable<Car> GetUserCarsByPage(Int32 currentPage, Int32 pageSize, Int32 userID);

        /// <summary>
        /// Получить автомобили пользователя по его имени
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        IEnumerable<Car> GetUserCars(String userName);
              
        /// <summary>
        /// Получить автомобили пользователя по его имени
        /// </summary>
        /// <param name="userID">Идентификатор пользователя</param>
        /// <param name="isOwnerRequest">true, если пользователь запрашивает свои авто</param>
        /// <returns></returns>
        IEnumerable<Car> GetUserCars(String userName, Boolean isOwnerRequest);

        /// <summary>
        /// Получить автомобили пользователя по его идентификатору
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        IEnumerable<Car> GetUserCars(Int32 userID);

        /// <summary>
        /// Получить автомобили пользователя по его идентификатору
        /// </summary>
        /// <param name="userID">Идентификатор пользователя</param>
        /// <param name="isOwnerRequest">true, если пользователь запрашивает свои авто</param>
        /// <returns></returns>
        IEnumerable<Car> GetUserCars(Int32 userID, Boolean isOwnerRequest);

        /// <summary>
        /// Получить количество автомобилей пользователя с указанным именем
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Int32 GetCountOfUserCars(String userName);

        /// <summary>
        /// Получить количество автомобилей пользователя с указанным именем идентификатором
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Int32 GetCountOfUserCars(Int32 userID);

        ///// <summary>
        ///// Определяет, является ли пользователь владельцем автомобиля,
        ///// нахдящегося в его гараже,
        ///// или этот авто был доверен ему
        ///// </summary>
        ///// <param name="carID">Идентификатор авто</param>
        ///// <param name="userName">Идентификатор пользователя</param>
        ///// <returns></returns>
        //UserAccesOnCar CheckUserAcces(int carID, String userName);

    }
}
