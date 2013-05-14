using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomasulo.Core
{
    public class ReservationItem
    {
        public string Time { get; set; }

        public string Name { get; set; }
        public string Busy { get; set; }

        public string Op { get; set; }

        public string F2 { get; set; }
        public string F3 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; }
    }
}
