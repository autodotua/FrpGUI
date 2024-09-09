using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Configs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Views;

public partial class LogPanel : UserControl
{
    private ScrollViewer lstScrollViewer;

    public LogPanel( )
    {
        DataContext = App.Services.GetRequiredService<LogViewModel>();
        InitializeComponent();
    }
}