using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomasulo.Core
{
    public class Instruction
    {
        public string Name;
        public int F1;
        public int F2;
        public int F3;

        public bool IsReady;
        public int TimeRemain;

        public int FinishTime;
        public int WriteBackTime;
    }
}
