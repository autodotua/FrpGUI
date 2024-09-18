using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Models;
using System;
using System.Diagnostics;

namespace FrpGUI.Avalonia.ViewModels;

[DebuggerDisplay("{Message}")]
public partial class LogInfo(LogEntity e) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUpdated))]
    public int updateTimes;

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