using Avalonia.Controls;
using Avalonia.Interactivity;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Config;
using System.Diagnostics.CodeAnalysis;

namespace FrpGUI.Avalonia.Views;

public partial class RuleWindow : Window
{
    public RuleWindow([NotNull] Rule oldRule)
    {
        DataContext = new RuleWindowViewModel() { Rule = oldRule.Clone() as Rule };
        InitializeComponent();
    }
    public RuleWindow()
    {
        DataContext = new RuleWindowViewModel();
        InitializeComponent();
    }
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as RuleWindowViewModel;
        if (vm.Check())
        {
            Close(vm.Rule);
        }
    }
}
