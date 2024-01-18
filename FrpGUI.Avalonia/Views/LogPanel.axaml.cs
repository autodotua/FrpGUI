using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Config;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FrpGUI.Avalonia.Views;

public partial class LogPanel : UserControl
{
    public LogPanel()
    {
        DataContext = new LogPanelViewModel();
        InitializeComponent();
    }

    private async void CopyMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var log = (sender as MenuItem).DataContext as UILog;
        await (VisualRoot as Window).Clipboard.SetTextAsync(log.Message);
    }
}