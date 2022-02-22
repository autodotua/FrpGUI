using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
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
            InitializeComponent();
        }

        public override void SetConfig(FrpConfigBase config)
        {
            base.SetConfig(config);
            Rules.Clear();
            (FrpConfig as ClientConfig).Rules.ForEach(p => Rules.Add(p));
            //Rules.CollectionChanged += (p1, p2) => (FrpConfig as ClientConfig).Rules = Rules.ToList();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
        }

        public ObservableCollection<Rule> Rules { get; } = new ObservableCollection<Rule>();
        protected override Button StartButton => btnStart;
        protected override Button StopButton => btnStop;
        protected override Button RestartButton => btnRestart;
        protected override Button CheckButton => btnCheck;

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            (FrpConfig as ClientConfig).Rules = Rules.ToList();
        }

        private void DataGrid_Selected(object sender, RoutedEventArgs e)
        {
            if ((sender as DataGrid).SelectedCells.First().Column.Header.Equals("类型"))
            {
                (sender as DataGrid).BeginEdit(e);
            }
        }

        private void AddRuleButton_Click(object sender, RoutedEventArgs e)
        {
            OpenAddRulePanel();

        }

        private void OpenAddRulePanel(Rule rule=null)
        {
            AddRulePanel panel = new AddRulePanel(rule);
            Grid grd = Content as Grid;
            panel.RequestClosing += (s, e) =>
            {
                if (panel.Save)
                {
                }
                Content = grd;

            };
            Content = panel;
        }

        private void ChangeRule_Click(object sender, RoutedEventArgs e)
        {
            OpenAddRulePanel((sender as FrameworkElement).DataContext as Rule);
        }
    }

    public class CellEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            NetType type = (NetType)value;
            return (parameter as string) switch
            {
                nameof(Rule.Domains) => type == NetType.HTTP || type == NetType.HTTPS,
                nameof(Rule.STCPKey) => type == NetType.STCP || type == NetType.STCP_Visitor,
                nameof(Rule.STCPServerName) => type == NetType.STCP_Visitor,
                nameof(Rule.RemotePort) => type != NetType.HTTP && type != NetType.HTTPS,
                _ => throw new ArgumentException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}