using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FrpGUI.Avalonia.DataProviders;
using FrpGUI.Avalonia.Views;
using FrpGUI.Configs;
using FrpGUI.Models;
using FzLib.Avalonia.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.ViewModels;
public partial class FrpConfigViewModel(IDataProvider provider, IServiceProvider services) : ViewModelBase(provider)
{
    [ObservableProperty]
    private IFrpProcess frp;

    [ObservableProperty]
    private ObservableCollection<Rule> rules;

    public async Task AddRuleAsync()
    {
        var dialog = services.GetRequiredService<RuleWindow>();
        var message = SendMessage(new DialogHostMessage(dialog));
        var result = await message.Task;
        if (result is Rule newRule)
        {
            Rules.Add(newRule);
        }
    }

    public void LoadConfig(IFrpProcess frp)
    {
        Frp = frp;
        if (frp.Config is ClientConfig cc)
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
        var dialog = services.GetRequiredService<RuleWindow>();
        dialog.SetRule(rule);
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
