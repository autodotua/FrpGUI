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

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        lstScrollViewer = lbx.GetVisualChildren().First().GetVisualChildren().First() as ScrollViewer;
        (DataContext as LogViewModel).Logs.CollectionChanged += Logs_CollectionChanged;
    }
    private async void Logs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        await Task.Delay(10); //不然视觉树还未形成，无法滚动到最下面
        try
        {
            Dispatcher.UIThread.Invoke(lstScrollViewer.ScrollToEnd);
        }
        catch (TaskCanceledException)
        {

        }
        catch (Exception ex)
        {

        }
    }

}