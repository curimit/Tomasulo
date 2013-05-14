using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomasulo.Core
{
    public class Instruction
    {
        public string Name { get; set; }
        public int F1 { get; set; }
        public int F2 { get; set; }
        public int F3 { get; set; }

        public int StartTime { get; set; }
        public int TimeRemain { get; set; }

        public int FinishTime { get; set; }
        public int WriteBackTime { get; set; }

        public Reservation reservation;
        public HashSet<int> Dependency = new HashSet<int>();
        public int MemoryLock = -1;

        public Action CallBack = null;

        public Instruction()
        {
            this.StartTime = -1;
        }
    }
}
