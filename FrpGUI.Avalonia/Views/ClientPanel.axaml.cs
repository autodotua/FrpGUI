using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Config;
using FzLib.Avalonia;
using System.Diagnostics;

namespace FrpGUI.Avalonia.Views;

public partial class ClientPanel : ConfigPanelBase
{
    public ClientPanel()
    {
        DataContext = new FrpConfigPanelViewModel();
        InitializeComponent();
    }

    public async void AddRule()
    {
        var dialog = new RuleWindow();
        var rule = await dialog.ShowDialog<Rule>(FzLib.Avalonia.Dialogs.DialogContainerType.PopupPreferred, this.GetWindow());
        if (rule != null)
        {
            (DataContext as FrpConfigPanelViewModel).Rules.Add(rule);
        }
    }

    private void DisableRuleMenu_Click(object sender, RoutedEventArgs e)
    {
        var rule = (sender as Visual).DataContext as Rule;
        Debug.Assert(rule != null);
        rule.Enable = true;
    }

    private void EnableRuleMenu_Click(object sender, RoutedEventArgs e)
    {
        var rule = (sender as Visual).DataContext as Rule;
        Debug.Assert(rule != null);
        rule.Enable = true;
    }

    private async void ModifyRuleMenu_Click(object sender, RoutedEventArgs e)
    {
        var rule = (sender as Visual).DataContext as Rule;
        Debug.Assert(rule != null);
        var dialog = new RuleWindow(rule);
        var newRule = await dialog.ShowDialog<Rule>(FzLib.Avalonia.Dialogs.DialogContainerType.PopupPreferred, this.GetVisualRoot() as Window);
        if (newRule != null)
        {
            var rules = (DataContext as FrpConfigPanelViewModel).Rules;
            rules[rules.IndexOf(rule)] = newRule;
        }
    }

    private void RemoveRuleMenu_Click(object sender, RoutedEventArgs e)
    {
        var rule = (sender as Visual).DataContext as Rule;
        Debug.Assert(rule != null);
        var rules = (DataContext as FrpConfigPanelViewModel).Rules;
        rules.Remove(rule);
    }

    private void PanelBase_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Resources["RuleWidth"] = lstRules.Bounds.Width switch
        {
            < 840 => lstRules.Bounds.Width - 0,
            _ => 420d
        };
    }
}