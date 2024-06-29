using Avalonia.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using FrpGUI.Avalonia.Views;
using FzLib;
using Avalonia.Threading;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FzLib.Avalonia.Messages;

namespace FrpGUI.Avalonia.ViewModels;

public partial class LogPanelViewModel : ViewModelBase
{
    public LogPanelViewModel()
    {
        Logger.NewLog += (s, e) => AddLog(e);
    }

    public ObservableCollection<LogViewModel> Logs { get; } = new ObservableCollection<LogViewModel>();

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
                    Logs[^i].UpdateTimes++;
                    return;
                }
            }
        }
        var log = new LogViewModel(e)
        {
            TypeBrush = brush,
        };
        Logs.Add(log);
    }

    [RelayCommand]
    private void CopyLog(LogViewModel log)
    {
        SendMessage(new GetClipboardMessage()).Clipboard.SetTextAsync(log.Message);
    }
}
