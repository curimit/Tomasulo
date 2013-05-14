using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomasulo.Core
{
    public class Config
    {
        public static List<Instruction> Example1 = new List<Instruction>
        {
            new Instruction{Name="LD", F1=6, F2=34},
            new Instruction{Name="LD", F1=2, F2=45},
            new Instruction{Name="MULD", F1=0, F2=2, F3=4},
            new Instruction{Name="SUBD", F1=0, F2=2, F3=4},
            new Instruction{Name="DIVD", F1=8, F2=6, F3=2},
            new Instruction{Name="ADDD", F1=6, F2=8, F3=2}
        };

        public static List<Instruction> Example2 = new List<Instruction>
        {
            new Instruction{Name="LD", F1=1, F2=34},
            new Instruction{Name="SD", F1=1, F2=0},
            new Instruction{Name="LD", F1=2, F2=0}
        };
    }
}
