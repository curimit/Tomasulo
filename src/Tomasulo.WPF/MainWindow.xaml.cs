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

            List<Order> orders = new List<Order>();
            orders.Add(new Order { Name = "LD", Desti = "F6", Sourcej = "34", Sourcek = "R2" });
            this.OrderQueue.ItemsSource = orders;
        }
    }

    public class Order
    {
        public string Name { get; set; }
        public string Desti { get; set; }
        public string Sourcej { get; set; }
        public string Sourcek { get; set; }
    }
}
