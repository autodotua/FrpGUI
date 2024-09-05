using Avalonia.Controls;
using Avalonia.Interactivity;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Configs;
using FzLib.Avalonia.Dialogs;
using System.Diagnostics.CodeAnalysis;

namespace FrpGUI.Avalonia.Views;

public partial class RuleWindow : DialogHost
{
    public RuleWindow([NotNull] Rule oldRule)
    {
        DataContext = new RuleViewModel() { Rule = oldRule.Clone() as Rule };
        InitializeComponent();
    }
    public RuleWindow()
    {
        DataContext = new RuleViewModel();
        InitializeComponent();
    }
    protected override void OnPrimaryButtonClick()
    {
        var vm = DataContext as RuleViewModel;
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
