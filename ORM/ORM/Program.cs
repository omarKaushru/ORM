using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ORM
{
    class Program
    {
        static void Main(string[] args)
        {
            using SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "Server = DESKTOP-8MG9JBI\\SQLEXPRESS; Database = Assignment; User Id = developer; Password = 2357;";
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            MyORM<House> myORM = new MyORM<House>(connection);

            var house = new House()
            {
                Id = 12,
                HouseName = "Shanti Niketon",
                HouseNo = "H-22",
                Rooms = new List<Room>
                {
                    new Room
                    {
                        Id =12, RoomColour ="Violate", RoomNo = 202,
                        Windows = new List <Window>
                        {
                           new Window { Id = 1223, WindowLength = 10, WindowWidth = 23},
                           new Window { Id = 1222, WindowLength = 11, WindowWidth = 20},
                           new Window { Id = 1221, WindowLength = 10, WindowWidth = 19}
                        }
                    },
                    new Room
                    {
                        Id =31, RoomColour ="Sky Blue", RoomNo = 12,
                        Windows = new List <Window>
                        {
                           new Window { Id = 1100, WindowLength = 9, WindowWidth = 23},
                           new Window { Id = 1101, WindowLength = 6, WindowWidth = 20},
                           new Window { Id = 1102, WindowLength = 7, WindowWidth = 19}
                        }
                    }
                }
            };
            myORM.Insert(house);
            //myORM.Update(house);
            //myORM.Delete(house);
        }
    }
}
