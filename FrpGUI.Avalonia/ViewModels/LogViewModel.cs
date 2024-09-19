using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrpGUI.Avalonia.DataProviders;
using FrpGUI.Models;
using FzLib.Avalonia.Messages;
using System;
using System.Collections.ObjectModel;

namespace FrpGUI.Avalonia.ViewModels;

public partial class LogViewModel : ViewModelBase
{
    private readonly UIConfig config;
    private readonly LocalLogger logger;

    [ObservableProperty]
    private LogInfo selectedLog;

    public LogViewModel(IDataProvider provider, UIConfig config, LocalLogger logger) : base(provider)
    {
        this.config = config;
        this.logger = logger;
        StartTimer();
    }

    public ObservableCollection<LogInfo> Logs { get; } = new ObservableCollection<LogInfo>();

    public void AddLog(LogEntity e)
    {
        try
        {
            //不知道为什么，加了内置浏览器后，Logs会报莫名其妙的错误，有时候Logs最后一个是null，
            //但是CollectionChanged里也不触发。所以加了最后一个null的判断以及try-catch
            while (Logs.Count > 0 && Logs[^1] == null)
            {
                Logs.RemoveAt(Logs.Count - 1);
            }
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
            SelectedLog = log;
        }
        catch (Exception ex)
        {

        }
    }

    [RelayCommand]
    private void CopyLog(LogInfo log)
    {
        SendMessage(new GetClipboardMessage()).Clipboard.SetTextAsync(log.Message);
    }

    private async void StartTimer()
    {
        if (DataProvider is WebDataProvider webDataProvider)
        {
            DateTime lastRequestTime = DateTime.MinValue;
            webDataProvider.AddTimerTask("获取日志", async () =>
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
            });
        }
        logger.NewLog += (s, e) => AddLog(e.Log);
    }
}