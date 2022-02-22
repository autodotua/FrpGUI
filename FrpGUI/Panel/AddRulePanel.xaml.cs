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
    public partial class AddRulePanel : UserControl
    {
        public AddRulePanel(Rule rule=null)
        {
            Rule = rule ?? new Rule();
            InitializeComponent();
            DataContext = this;
        }
        public Rule Rule { get; set; }
        public bool Save { get; set; }
        public event EventHandler RequestClosing;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save = true;
            RequestClosing?.Invoke(this, e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RequestClosing?.Invoke(this, e);
        }
    }

}