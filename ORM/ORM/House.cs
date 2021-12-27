using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM
{
    public class House :IData
    {
        public int Id { get; set; }
        public string HouseName { get; set; }
        public string HouseNo { get; set; }
        public IList<Room> Rooms { get; set;}
    }
}
