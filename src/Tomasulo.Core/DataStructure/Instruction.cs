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

        public int 执行指令;

        public int 发射指令 { get; set; }
        public int 剩余时间 { get; set; }

        public int 完成时间 { get; set; }
        public int 写回时间 { get; set; }

        public Reservation reservation;
        public HashSet<int> Dependency = new HashSet<int>();
        public int MemoryLock = -1;

        public Action ReadyBack = null;
        public Action CallBack = null;

        public Instruction()
        {
            this.执行指令 = -1;
        }
    }
}
