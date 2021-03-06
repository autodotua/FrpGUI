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
    public partial class ClientPanel : PanelBase
    {
        public ClientPanel()
        {
            Rules = new ObservableCollection<Rule>(Client.Rules);
            Rules.CollectionChanged += (p1, p2) => Client.Rules = Rules.ToList();
            InitializeComponent();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
        }

        protected override void ChangeStatus(ProcessStatus status)
        {
            base.ChangeStatus(status);
            if (status == ProcessStatus.Running)
            {
                Config.Instance.ClientOn = true;
            }
            else
            {
                Config.Instance.ClientOn = false;
            }
        }

        public ClientConfig Client => Config.Instance.Client;
        public ObservableCollection<Rule> Rules { get; }
        protected override Button StartButton => btnStart;
        protected override Button StopButton => btnStop;
        protected override Button RestartButton => btnRestart;
        protected override string Type => "c";
        protected override IToIni ConfigItem => Client;

        protected override ProcessHelper Process { get; } = ProcessHelper.Client;

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Client.Rules = Rules.ToList();
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