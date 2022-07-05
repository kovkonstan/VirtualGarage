using System;
using System.Linq;
using InostudioSolutions.Data;
using VirtualGarage.Logic.DataModel;
using System.Collections.Generic;
using VirtualGarage.Logic.Enums;

namespace VirtualGarage.Logic.Repository
{
    public interface ICarRepo : IRepository<Car>
    {
        /// <summary>
        /// ���������� ����� ������������ �� ����������
        /// </summary>
        /// <param name="carID">������������� ����������</param>
        /// <param name="userName">��� ������������</param>
        /// <returns></returns>
        UserAccesOnCar CheckUserAcces(int carID, String userName);

        /// <summary>
        /// ���������� ����������, ���������� ������� �������� ��������� ������������
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<Car> UserOwnedCars(Int32 userId);

        /// <summary>
        /// ���������� ��� ���������� � ������ ������������(� �.� ����������)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<Car> CarsInUserGarage(Int32 userId);

        /// <summary>
        /// ���������� ������� ����������
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        IEnumerable<Car> GetSimilarCars(Int32 carID);
    }
}