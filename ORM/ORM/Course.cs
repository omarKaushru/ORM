﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM
{
    public class Course : IData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CourseFee { get; set; }
        public string Availability { get; set; }
    }
}
