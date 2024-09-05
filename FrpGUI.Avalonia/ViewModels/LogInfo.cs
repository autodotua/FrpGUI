using Avalonia.Media;
using System.Diagnostics;
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FrpGUI.Avalonia.ViewModels;

[DebuggerDisplay("{Message}")]
public partial class LogInfo(LogEventArgs e) :ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUpdated))]
    public int updateTimes;

    [ObservableProperty]
    private Exception exception = e.Exception;

    [ObservableProperty]
    private bool fromFrp = e.FromFrp;

    [ObservableProperty]
    private string instanceName = e.InstanceName;

    [ObservableProperty]
    private string message = e.Message;

    [ObservableProperty]
    private DateTime time = e.Time;

    [ObservableProperty]
    private char type = e.Type;

    [ObservableProperty]
    private IBrush typeBrush;
    public bool HasUpdated => UpdateTimes > 0;
}