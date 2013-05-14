using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomasulo.Core
{
    public class Simulator
    {
        public List<Instruction> Q = new List<Instruction>();
        public int PC;

        public double[] Memory = new double[4096];
        public FU[] Fu = new FU[11];

        public int Time;

        public bool IsComplete;

        public void Reset()
        {
            PC = 0;
            for (int i = 0; i < Memory.Length; i++)
            {
                Memory[i] = 0;
            }
            for (int i = 0; i < Fu.Length; i++)
            {
                Fu[i] = new FU();
            }
            Q.Clear();
        }

        public void Start()
        {
            IsComplete = false;
            Time = 0;
            Next();
        }

        public void Next()
        {
            if (IsComplete) return;

            // Run Next Instruction
            if (PC < Q.Count())
            {
                Instruction I = Q[PC++];
                switch (I.Name)
                {
                    case "LD":
                        {

                            break;
                        }
                }
            }
        }
    }
}
