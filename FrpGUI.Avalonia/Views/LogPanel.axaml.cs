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
        (DataContext as LogPanelViewModel).Logs.CollectionChanged += Logs_CollectionChanged;
    }

    private void Logs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        var list = sender as ObservableCollection<UILog>;
        Debug.Assert(list != null);
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            if (lbx.SelectedItem == null || lbx.SelectedItem == list[^2])
            {
                lbx.SelectedItem = list[^1];
                lbx.ScrollIntoView(lbx.SelectedItem);
            }
        }
    }

    private async void CopyMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var log = (sender as MenuItem).DataContext as UILog;
        await (VisualRoot as Window).Clipboard.SetTextAsync(log.Message);
    }
}