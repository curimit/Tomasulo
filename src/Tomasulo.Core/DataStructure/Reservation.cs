using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomasulo.Core
{
    public class Reservation
    {
        public int Time { get; set; }
        public string Name { get; set; }
        public bool IsBusy { get; set; }
        public string Op { get; set; }

        public double F2 { get; set; }
        public double F3 { get; set; }

        public string Q2 { get; set; }
        public string Q3 { get; set; }
    }
}
