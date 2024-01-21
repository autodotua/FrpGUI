using Avalonia.Media;
using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Collections.ObjectModel;
using FrpGUI.Avalonia.Views;
using FzLib;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FrpGUI.Avalonia.ViewModels;

public partial class LogPanelViewModel : ViewModelBase
{
    public LogPanelViewModel()
    {
        Logger.NewLog += (s, e) => AddLog(e);
    }

    public ObservableCollection<UILog> Logs { get; } = new ObservableCollection<UILog>();

    [ObservableProperty]
    private UILog selectedLog;

    public void AddLog(LogEventArgs e)
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
                    Logs[^i].UpdateTime();
                    return;
                }
            }
        }
        Dispatcher.UIThread.Invoke(() =>
        {
            bool needScroll = Logs.Count == 0 || SelectedLog == Logs[^1] || SelectedLog == null;
            var log = new UILog(e)
            {
                TypeBrush = brush,
            };
            Logs.Add(log);
            if (needScroll)
            {
                SelectedLog = log;
            }
        });
    }

}

[DebuggerDisplay("{Message}")]
public class UILog(LogEventArgs e) : LogEventArgs(e.Message, e.InstanceName, e.Type, e.FromFrp, e.Exception), INotifyPropertyChanged
{
    private DateTime time = e.Time;
    private IBrush typeBrush;
    public event PropertyChangedEventHandler PropertyChanged;

    public DateTime ChangeableTime
    {
        get => time;
        private set => this.SetValueAndNotify(ref time, value, nameof(ChangeableTime));
    }

    public bool HasUpdated => UpdateTimes > 0;

    public IBrush TypeBrush
    {
        get => typeBrush;
        set => this.SetValueAndNotify(ref typeBrush, value, nameof(TypeBrush));
    }

    public int UpdateTimes { get; set; }

    public void UpdateTime()
    {
        UpdateTimes++;
        this.Notify(nameof(UpdateTimes), nameof(HasUpdated));
    }
}