using Avalonia.Controls;
using Avalonia.Interactivity;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Config;
using FzLib.Avalonia.Dialogs;
using System.Diagnostics.CodeAnalysis;

namespace FrpGUI.Avalonia.Views;

public partial class RuleWindow : DialogHost
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
    protected override void OnPrimaryButtonClick()
    {
        var vm = DataContext as RuleWindowViewModel;
        if (vm.Check())
        {
            Close(vm.Rule);
        }
    }

    protected override void OnCloseButtonClick()
    {
        Close();
    }
}
