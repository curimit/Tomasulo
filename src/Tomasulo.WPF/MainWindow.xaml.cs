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

namespace Tomasulo.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            List<InstructionItem> orders = new List<InstructionItem>();
            orders.Add(new InstructionItem { Name = "LD", F1 = "F6", F2 = "34", F3 = "R2" });
            this.OrderQueue.ItemsSource = orders;
        }
    }
}
