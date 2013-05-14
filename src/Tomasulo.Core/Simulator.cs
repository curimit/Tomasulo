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
        public bool[] MemoryBusy = new bool[4096];
        public FU[] Fu = new FU[11];

        public Reservation[] Add = new Reservation[3];
        public Reservation[] Mult = new Reservation[2];

        public LoadItem[] LQ = new LoadItem[3];
        public StoreItem[] SQ = new StoreItem[3];

        public int Time;

        public bool IsComplete;

        public void Reset()
        {
            PC = 0;
            for (int i = 0; i < Memory.Length; i++)
            {
                Memory[i] = 0;
                MemoryBusy[i] = false;
            }
            Memory[34] = 123;
            for (int i = 0; i < Fu.Length; i++)
            {
                Fu[i] = new FU();
                Fu[i].Value = i;
            }
            for (int i = 0; i < LQ.Length; i++)
            {
                LQ[i] = new LoadItem();
                LQ[i].Header = string.Format("Load{0}", i + 1);
                LQ[i].Address = -1;
            }

            for (int i = 0; i < SQ.Length; i++)
            {
                SQ[i] = new StoreItem();
                SQ[i].Header = string.Format("Store{0}", i + 1);
                SQ[i].Address = -1;
            }

            for (int i = 0; i < Add.Length; i++)
            {
                Add[i] = new Reservation();
                Add[i].Time = 0;
                Add[i].Name = string.Format("Add{0}", i + 1);
            }

            for (int i = 0; i < Mult.Length; i++)
            {
                Mult[i] = new Reservation();
                Mult[i].Time = 0;
                Mult[i].Name = string.Format("Mult{0}", i + 1);
            }

            Q = new List<Instruction>();
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

            Time++;

            // Run Next Instruction
            if (PC < Q.Count()) PC++;

            // Each Instruction Past 1 Tick
            foreach (var I in Q.Take(PC).Where(x => x.StartTime != -1))
            {
                if (I.WriteBackTime != -1) continue;

                if (I.FinishTime != -1)
                {
                    I.WriteBackTime = Time;
                    if (I.CallBack != null) I.CallBack();
                    continue;
                }

                if (I.Dependency.Count() > 0 || I.MemoryLock != -1)
                {
                    continue;
                }

                I.TimeRemain--;

                if (I.TimeRemain == 0)
                {
                    I.FinishTime = Time;
                }

                if (I.reservation != null) I.reservation.Time--;
            }

            // Dependency
            foreach (var I in Q.Take(PC).Where(x => x.StartTime != -1))
            {
                if (I.reservation != null)
                {
                    if (I.reservation.Q2 != null && Fu[I.F2].Expr == null)
                    {
                        I.reservation.Q2 = null;
                        I.reservation.F2 = Fu[I.F2].Value;
                    }
                    if (I.reservation.Q3 != null && Fu[I.F3].Expr == null)
                    {
                        I.reservation.Q3 = null;
                        I.reservation.F3 = Fu[I.F3].Value;
                    }
                }
                foreach (var x in I.Dependency.ToList())
                {
                    if (Fu[x].Expr == null)
                    {
                        I.Dependency.Remove(x);
                    }
                }
                if (I.MemoryLock != -1 && !this.MemoryBusy[I.MemoryLock])
                {
                    I.MemoryLock = -1;
                }
            }

            // Check Dependency Of Instructions
            foreach (var I in Q.Take(PC).Where(x => x.StartTime == -1))
            {
                switch (I.Name)
                {
                    case "LD":
                        {
                            var item = LQ.ToList().Find(x => !x.IsBusy);
                            item.IsBusy = true;
                            item.Address = I.F2;

                            I.StartTime = Time;
                            I.TimeRemain = 2;
                            I.FinishTime = I.WriteBackTime = -1;
                            I.reservation = null;
                            
                            if (MemoryBusy[I.F2])
                            {
                                I.MemoryLock = I.F2;
                            }

                            Fu[I.F1].Expr = item.Header;

                            I.CallBack = () =>
                            {
                                item.IsBusy = false;
                                Fu[I.F1].Value = Memory[I.F2];
                                Fu[I.F1].Expr = null;
                                item.Address = -1;
                            };
                            break;
                        }

                    case "SD":
                        {
                            var item = SQ.ToList().Find(x => !x.IsBusy);
                            item.IsBusy = true;
                            item.Address = I.F2;

                            I.StartTime = Time;
                            I.TimeRemain = 2;
                            I.FinishTime = I.WriteBackTime = -1;
                            I.reservation = null;
                            if (Fu[I.F1].Expr != null)
                            {
                                I.Dependency.Add(I.F1);
                            }

                            MemoryBusy[I.F2] = true;
                            I.CallBack = () =>
                            {
                                item.IsBusy = false;
                                Memory[I.F2] = Fu[I.F1].Value;
                                MemoryBusy[I.F2] = false;
                                item.Address = -1;
                            };
                            break;
                        }

                    case "ADDD":
                        {
                            var item = Add.ToList().Find(x => !x.IsBusy);
                            if (item == null) continue;

                            item.IsBusy = true;
                            item.Op = "ADDD";

                            I.StartTime = Time;
                            I.TimeRemain = 2;
                            I.FinishTime = I.WriteBackTime = -1;
                            I.reservation = item;

                            if (Fu[I.F2].Expr != null)
                            {
                                I.Dependency.Add(I.F2);
                            }
                            if (Fu[I.F3].Expr != null)
                            {
                                I.Dependency.Add(I.F3);
                            }

                            item.Time = I.TimeRemain;

                            item.Q2 = Fu[I.F2].Expr;
                            item.Q3 = Fu[I.F3].Expr;
                            if (item.Q2 == null)
                            {
                                item.F2 = Fu[I.F2].Value;
                            }
                            if (item.Q3 == null)
                            {
                                item.F3 = Fu[I.F3].Value;
                            }

                            Fu[I.F1].Expr = string.Format(item.Name, I.F2, I.F3);

                            I.CallBack = () =>
                            {
                                item.IsBusy = false;
                                item.Op = null;
                                Fu[I.F1].Value = Fu[I.F2].Value + Fu[I.F3].Value;
                                Fu[I.F1].Expr = null;
                            };
                            break;
                        }

                    case "SUBD":
                        {
                            var item = Add.ToList().Find(x => !x.IsBusy);
                            if (item == null) continue;

                            item.IsBusy = true;
                            item.Op = "SUBD";

                            I.StartTime = Time;
                            I.TimeRemain = 2;
                            I.FinishTime = I.WriteBackTime = -1;
                            I.reservation = item;

                            if (Fu[I.F2].Expr != null)
                            {
                                I.Dependency.Add(I.F2);
                            }
                            if (Fu[I.F3].Expr != null)
                            {
                                I.Dependency.Add(I.F3);
                            }

                            item.Time = I.TimeRemain;

                            item.Q2 = Fu[I.F2].Expr;
                            item.Q3 = Fu[I.F3].Expr;
                            if (item.Q2 == null)
                            {
                                item.F2 = Fu[I.F2].Value;
                            }
                            if (item.Q3 == null)
                            {
                                item.F3 = Fu[I.F3].Value;
                            }

                            Fu[I.F1].Expr = string.Format(item.Name, I.F2, I.F3);

                            I.CallBack = () =>
                            {
                                item.IsBusy = false;
                                item.Op = null;
                                Fu[I.F1].Value = Fu[I.F2].Value - Fu[I.F3].Value;
                                Fu[I.F1].Expr = null;
                            };
                            break;
                        }

                    case "MULD":
                        {
                            var item = Mult.ToList().Find(x => !x.IsBusy);
                            if (item == null) continue;

                            item.IsBusy = true;
                            item.Op = "MULD";

                            I.StartTime = Time;
                            I.TimeRemain = 10;
                            I.FinishTime = I.WriteBackTime = -1;
                            I.reservation = item;

                            if (Fu[I.F2].Expr != null)
                            {
                                I.Dependency.Add(I.F2);
                            }
                            if (Fu[I.F3].Expr != null)
                            {
                                I.Dependency.Add(I.F3);
                            }

                            item.Time = I.TimeRemain;

                            item.Q2 = Fu[I.F2].Expr;
                            item.Q3 = Fu[I.F3].Expr;
                            if (item.Q2 == null)
                            {
                                item.F2 = Fu[I.F2].Value;
                            }
                            if (item.Q3 == null)
                            {
                                item.F3 = Fu[I.F3].Value;
                            }

                            Fu[I.F1].Expr = string.Format(item.Name, I.F2, I.F3);

                            I.CallBack = () =>
                            {
                                item.IsBusy = false;
                                item.Op = null;
                                Fu[I.F1].Value = Fu[I.F2].Value * Fu[I.F3].Value;
                                Fu[I.F1].Expr = null;
                            };
                            break;
                        }

                    case "DIVD":
                        {
                            var item = Mult.ToList().Find(x => !x.IsBusy);
                            if (item == null) continue;

                            item.IsBusy = true;
                            item.Op = "DIVD";

                            I.StartTime = Time;
                            I.TimeRemain = 40;
                            I.FinishTime = I.WriteBackTime = -1;
                            I.reservation = item;

                            if (Fu[I.F2].Expr != null)
                            {
                                I.Dependency.Add(I.F2);
                            }
                            if (Fu[I.F3].Expr != null)
                            {
                                I.Dependency.Add(I.F3);
                            }

                            item.Time = I.TimeRemain;

                            item.Q2 = Fu[I.F2].Expr;
                            item.Q3 = Fu[I.F3].Expr;
                            if (item.Q2 == null)
                            {
                                item.F2 = Fu[I.F2].Value;
                            }
                            if (item.Q3 == null)
                            {
                                item.F3 = Fu[I.F3].Value;
                            }

                            Fu[I.F1].Expr = string.Format(item.Name, I.F2, I.F3);

                            I.CallBack = () =>
                            {
                                item.IsBusy = false;
                                item.Op = null;
                                Fu[I.F1].Value = Fu[I.F2].Value / Fu[I.F3].Value;
                                Fu[I.F1].Expr = null;
                            };
                            break;
                        }
                }
            }

            // Check Complete
            IsComplete = PC == Q.Count() && Q.All(x => x.WriteBackTime != -1);
        }
    }
}
