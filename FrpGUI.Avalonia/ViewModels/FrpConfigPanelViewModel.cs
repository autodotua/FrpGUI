using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FrpGUI.Avalonia.Views;
using FrpGUI.Config;
using FzLib.Avalonia.Messages;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.ViewModels;
public partial class FrpConfigPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    private FrpConfigBase frpConfig;

    [ObservableProperty]
    private ObservableCollection<Rule> rules;

    public async Task AddRuleAsync()
    {
        var dialog = new RuleWindow();
        var message = SendMessage(new DialogHostMessage(dialog));
        var result = await message.Task;
        if (result is Rule newRule)
        {
            Rules.Add(newRule);
        }
    }

    public void LoadConfig(FrpConfigBase frpConfig)
    {
        FrpConfig = frpConfig;
        if (frpConfig is ClientConfig cc)
        {
            Rules = new ObservableCollection<Rule>(cc.Rules);
            Rules.CollectionChanged += (s, e) => cc.Rules = [.. Rules];
        }
    }

    [RelayCommand]
    private void DisableRule(Rule rule)
    {
        rule.Enable = false;
    }

    [RelayCommand]
    private void EnableRule(Rule rule)
    {
        rule.Enable = true;
    }

    [RelayCommand]
    private async Task ModifyRuleAsync(Rule rule)
    {
        var dialog = new RuleWindow(rule);
        var message = SendMessage(new DialogHostMessage(dialog));
        var result = await message.Task;
        if (result is Rule newRule)
        {
            Rules[Rules.IndexOf(rule)] = newRule;
        }
    }

    [RelayCommand]
    private void RemoveRule(Rule rule)
    {
        Rules.Remove(rule);
    }
}
