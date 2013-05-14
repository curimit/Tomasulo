using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tomasulo.Core;
using Fluent;
using System.Windows.Threading;

namespace Tomasulo.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private Simulator simulator = new Simulator();
        private FuItem FuExpr = new FuItem();
        private FuItem FuValue = new FuItem();
        private LoadItem[] LQ = new LoadItem[3];
        private StoreItem[] SQ = new StoreItem[3];

        private List<MemItem> Memory = new List<MemItem>();

        private Reservation[] reservation = new Reservation[5];


        private int Delay = 20;
        private int Delay_Counter = 0;

        private bool AutoNext = false;

        public MainWindow()
        {
            InitializeComponent();

            this.MemoryTable.ItemsSource = this.Memory;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(100);
            timer.Tick += timer_Tick;
            timer.Start();

            Reset();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Delay_Counter = (Delay_Counter + 1) % Delay;
            if (Delay_Counter == 0 && AutoNext && !this.simulator.IsComplete)
            {
                this.Next_Click(null, null);
            }
        }

        private void Reset()
        {
            this.FuTable.ItemsSource = new List<FuItem> { FuExpr, FuValue };
            this.Memory.Clear();
            this.MemoryTable.Items.Refresh();

            for (int i = 0; i < 3; i++)
            {
                LQ[i] = new LoadItem();
                LQ[i].Header = string.Format("Load{0}", i + 1);

                SQ[i] = new StoreItem();
                SQ[i].Header = string.Format("Store{0}", i + 1);
            }
            this.LoadQueue.ItemsSource = this.LQ;
            this.StoreQueue.ItemsSource = this.SQ;

            for (int i = 0; i < 5; i++)
            {
                reservation[i] = new Reservation();
            }
            this.ReservationTable.ItemsSource = this.reservation;

            this.Time.Content = "0";
            this.PC.Content = "0";

            this.simulator.Reset();

            this.OrderQueue.ItemsSource = this.simulator.Q;
            this.OrderQueue.Items.Refresh();
        }

        private void Example1_Click(object sender, RoutedEventArgs e)
        {
            this.Reset();

            this.simulator.Q = Config.Example1;

            this.Length.Content = simulator.Q.Count().ToString();

            this.OrderQueue.ItemsSource = simulator.Q;

            this.AutoNext = false;
        }

        private void Example2_Click(object sender, RoutedEventArgs e)
        {
            this.Reset();

            this.simulator.Q = Config.Example2;

            this.Length.Content = simulator.Q.Count().ToString();

            this.OrderQueue.ItemsSource = simulator.Q;

            this.AutoNext = false;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (simulator.IsComplete)
            {
                MessageBox.Show("指令执行完毕！");
            }

            simulator.Next();

            UpdateGui();
        }

        private void UpdateGui()
        {

            this.PC.Content = simulator.PC.ToString();
            this.Time.Content = simulator.Time.ToString();

            // Update Fu
            for (int i = 0; i <= 10; i++)
            {
                var info = typeof(FuItem).GetProperty(string.Format("F{0}", i));
                info.SetValue(FuExpr, simulator.Fu[i].Expr, null);
                info.SetValue(FuValue, simulator.Fu[i].Value.ToString(), null);
            }
            this.FuTable.Items.Refresh();

            // Update Load Queue
            for (int i = 0; i < simulator.LQ.Length; i++)
            {
                this.LQ[i] = simulator.LQ[i];
            }
            this.LoadQueue.Items.Refresh();

            //Update Store Queue
            for (int i = 0; i < simulator.LQ.Length; i++)
            {
                this.SQ[i] = simulator.SQ[i];
            }
            this.StoreQueue.Items.Refresh();

            // Update Reservation
            this.reservation[0] = simulator.Add[0];
            this.reservation[1] = simulator.Add[1];
            this.reservation[2] = simulator.Add[2];
            this.reservation[3] = simulator.Mult[0];
            this.reservation[4] = simulator.Mult[1];
            this.ReservationTable.Items.Refresh();

            // Update Order Queue
            this.OrderQueue.Items.Refresh();

            // Update Memory Table
            foreach (var x in this.Memory)
            {
                try
                {
                    int k = int.Parse(x.Address);
                    x.Value = this.simulator.Memory[k].ToString();
                }
                catch
                {
                }
            }
            this.MemoryTable.Items.Refresh();
        }

        private void MemoryTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            System.Windows.Controls.TextBox t = e.EditingElement as System.Windows.Controls.TextBox;

            string Option = e.Column.SortMemberPath;
            var item = this.Memory[this.MemoryTable.SelectedIndex];
            if (Option == "Address")
            {
                item.Address = t.Text;
                item.Value = this.simulator.Memory[int.Parse(t.Text)].ToString();
            }
            else
            {
                this.simulator.Memory[int.Parse(item.Address)] = double.Parse(t.Text);
                item.Value = t.Text;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            int k = this.OrderQueue.SelectedIndex + 1;
            if (k == -1) k = 0;
            this.simulator.Q.Insert(k, new Instruction());
            this.OrderQueue.Items.Refresh();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int k = this.OrderQueue.SelectedIndex;
            if (k == -1) return;
            this.simulator.Q.RemoveAt(k);
            this.OrderQueue.Items.Refresh();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (simulator.IsComplete)
            {
                MessageBox.Show("指令执行完毕！");
            }

            this.simulator.Start();
            UpdateGui();

            this.AutoNext = false;
        }

        private void Auto_Click(object sender, RoutedEventArgs e)
        {
            if (simulator.IsComplete)
            {
                MessageBox.Show("指令执行完毕！");
            }

            this.simulator.Start();
            UpdateGui();

            this.AutoNext = true;
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            this.AutoNext = false;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.AutoNext = false;
            this.simulator.Clear();
            UpdateGui();
        }
    }
}
