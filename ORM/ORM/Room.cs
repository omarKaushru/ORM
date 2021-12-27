using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM
{
    public class Room : IData
    {
        public int Id { get; set; }
        public string RoomColour { get; set; }
        public int RoomNo { get; set; }
        public IList<Window> Windows { get; set; }
    }
}
