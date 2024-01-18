using Avalonia.Media;
using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Collections.ObjectModel;
using FrpGUI.Avalonia.Views;
using FzLib;
using Avalonia.Threading;

namespace FrpGUI.Avalonia.ViewModels;

public partial class LogPanelViewModel : ViewModelBase
{
    public LogPanelViewModel()
    {
        Logger.NewLog += (s, e) => AddLog(e);
    }

    public ObservableCollection<UILog> Logs { get; } = new ObservableCollection<UILog>();

    public void AddLog(LogEventArgs e)
    {
        IBrush brush = Brushes.Green;
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
            Logs.Add(new UILog(e)
            {
                TypeBrush = brush,
            });
        });
        //if (Logs.Count > 0)
        //{
        //    while (Logs.Count > MaxLogCount)
        //    {
        //        Logs.RemoveAt(0);
        //    }
        //}
    }

}

[DebuggerDisplay("{Message}")]
public class UILog(LogEventArgs e) : LogEventArgs(e.Message, e.InstanceName, e.Type, e.FromFrp), INotifyPropertyChanged
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