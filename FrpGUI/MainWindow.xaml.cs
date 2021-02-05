using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrpGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ProcessHelper.Output += ProcessHelper_Output;
        }

        private void ProcessHelper_Output(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                lbxLogs.Items.Add(e.Data);
                lbxLogs.ScrollIntoView(lbxLogs.Items[^1]);
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}