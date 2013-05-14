using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomasulo.Core
{
    public class InstructionItem
    {
        public string Name { get; set; }
        public string F1 { get; set; }
        public string F2 { get; set; }
        public string F3 { get; set; }

        public string IsReady { get; set; }
        public string TimeRemain { get; set; }

        public string FinishTime { get; set; } 
        public string WriteBackTime { get; set; } 
    }
}
