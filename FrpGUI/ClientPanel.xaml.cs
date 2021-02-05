using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace FrpGUI
{
    /// <summary>
    /// ServerPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ClientPanel : UserControl
    {
        private ProcessHelper process = new ProcessHelper();

        public ClientPanel()
        {
            InitializeComponent();
            DataContext = this;
            process.Exited += Process_Exited;
            Rules = new ObservableCollection<Rule>(Client.Rules);

            SetUIEnable(false);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            SetUIEnable(false);
        }

        private void SetUIEnable(bool running)
        {
            Dispatcher.Invoke(() =>
            {
                btnStart.IsEnabled = !running;
                btnRestart.IsEnabled = running;
                btnStop.IsEnabled = running;
            });
        }

        public ClientConfig Client => Config.Instance.Client;
        public ObservableCollection<Rule> Rules { get; set; }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            SetUIEnable(true);
            Client.Rules = Rules.ToList();
            process.Start("c", Client);
            Config.Instance.Save();
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            process.Restart();
            SetUIEnable(true);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            process.Stop();
            SetUIEnable(false);
        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
        }
    }

    public static class CustomColumnHeadersProperty
    {
        public static DependencyProperty ItemTypeProperty = DependencyProperty.RegisterAttached(
            "ItemType",
            typeof(Type),
            typeof(CustomColumnHeadersProperty),
            new PropertyMetadata(OnItemTypeChanged));

        public static void SetItemType(DependencyObject obj, Type value)
        {
            obj.SetValue(ItemTypeProperty, value);
        }

        public static Type GetItemType(DependencyObject obj)
        {
            return (Type)obj.GetValue(ItemTypeProperty);
        }

        private static void OnItemTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = sender as DataGrid;

            if (args.NewValue != null)
                dataGrid.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            else
                dataGrid.AutoGeneratingColumn -= dataGrid_AutoGeneratingColumn;
        }

        private static void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var type = GetItemType(sender as DataGrid);

            var displayAttribute = type.GetProperty(e.PropertyName).GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            if (displayAttribute != null)
                e.Column.Header = displayAttribute.Name;
        }
    }
}