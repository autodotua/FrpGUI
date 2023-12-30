using FrpGUI.Config;
using FzLib.WPF;
using Mapster;
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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrpGUI.WPF
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
        protected override Control ConfigView => scrConfig;

        private void AddRuleButton_Click(object sender, RoutedEventArgs e)
        {
            OpenAddRulePanel();

        }

        private void OpenAddRulePanel(Rule rule=null)
        {
            AddRulePanel panel = new AddRulePanel(Rules ,rule);
            Grid grd = Content as Grid;
            panel.RequestClosing += (s, e) =>
            {
                if (panel.Save)
                {
                    if (rule == null)
                    {
                        Rules.Add(panel.Rule);
                    }
                    else
                    {
                        panel.Rule.Adapt(rule);
                    }
                    SaveRules();
                }
                Content = grd;

            };
            Content = panel;
        }

        private void SaveRules()
        {
            (FrpConfig as ClientConfig).Rules = Rules.ToList();
        }

        private void ChangeRule_Click(object sender, RoutedEventArgs e)
        {
            OpenAddRulePanel((sender as FrameworkElement).DataContext as Rule);
        }

        private void DeleteRule_Click(object sender, RoutedEventArgs e)
        {
            Rules.Remove((sender as FrameworkElement).DataContext as Rule);
            SaveRules();
        }

        private void EnableRule_Click(object sender, RoutedEventArgs e)
        {
            SaveRules();
        }

        private void PanelBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resources["RuleWidth"] = lstRules.ActualWidth switch
            {
                < 840 => lstRules.ActualWidth-0 ,
                _ => 420d
            };
        }
    }
}