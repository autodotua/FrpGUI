using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FrpGUI.Avalonia.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace FrpGUI.Avalonia.Views;

public partial class LogPanel : UserControl
{
    private ScrollViewer lstScrollViewer;

    public LogPanel()
    {
        DataContext = App.Services.GetRequiredService<LogViewModel>();
        InitializeComponent();
    }
}