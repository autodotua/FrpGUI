using Avalonia.Controls;
using Avalonia.Interactivity;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Configs;
using FzLib.Avalonia.Dialogs;
using System.Diagnostics.CodeAnalysis;

namespace FrpGUI.Avalonia.Views;

public partial class RuleDialog : DialogHost
{
    public RuleDialog(RuleViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }

    public void SetRule(Rule rule)
    {
        (DataContext as RuleViewModel).Rule = rule.Clone() as Rule;
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
