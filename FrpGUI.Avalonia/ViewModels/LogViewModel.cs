using Avalonia.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using FrpGUI.Avalonia.Views;
using FzLib;
using Avalonia.Threading;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FzLib.Avalonia.Messages;
using System;
using FrpGUI.Avalonia.DataProviders;
using System.Threading;
using FrpGUI.Models;

namespace FrpGUI.Avalonia.ViewModels;

public partial class LogViewModel : ViewModelBase
{
    public LogViewModel(IDataProvider provider) : base(provider)
    {
        StartTimer();
    }

    private async void StartTimer()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
        DateTime lastRequestTime = DateTime.MinValue;
        while (await timer.WaitForNextTickAsync())
        {
            var logs = await DataProvider.GetLogsAsync(lastRequestTime);
            if (logs.Count > 0)
            {
                lastRequestTime = logs[^1].Time;
                foreach (var log in logs)
                {
                    AddLog(log);
                }
            }
        }
    }

    public ObservableCollection<LogInfo> Logs { get; } = new ObservableCollection<LogInfo>();

    public void AddLog(LogEntity e)
    {
        IBrush brush = Brushes.Transparent;
        if (e.Type == 'W')
        {
            brush = Brushes.Orange;
        }
        else if (e.Type == 'E')
        {
            brush = Brushes.Red;
        }

        if (Logs.Count >= 2)
        {
            for (int i = 1; i <= 2; i++)
            {
                if (Logs[^i].Message == e.Message)
                {
                    Logs[^i].UpdateTimes++;
                    return;
                }
            }
        }
        var log = new LogInfo(e)
        {
            TypeBrush = brush,
        };
        Logs.Add(log);
    }

    [RelayCommand]
    private void CopyLog(LogInfo log)
    {
        SendMessage(new GetClipboardMessage()).Clipboard.SetTextAsync(log.Message);
    }
}
