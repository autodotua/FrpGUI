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
using System.Linq;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Views;

public partial class LogPanel : UserControl
{
    public LogPanel()
    {
        DataContext = new LogPanelViewModel();
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        lstScrollViewer = lbx.GetVisualChildren().First().GetVisualChildren().First() as ScrollViewer;
        (DataContext as LogPanelViewModel).Logs.CollectionChanged += Logs_CollectionChanged;
    }

    private ScrollViewer lstScrollViewer;

    private async void Logs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        await Task.Delay(10); //不然视觉树还未形成，无法滚动到最下面
        lstScrollViewer.ScrollToEnd();
    }

    private async void CopyMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var log = (sender as MenuItem).DataContext as UILog;
        await (VisualRoot as Window).Clipboard.SetTextAsync(log.Message);
    }

}