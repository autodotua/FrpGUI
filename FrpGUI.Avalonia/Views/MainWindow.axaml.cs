using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FrpGUI.Avalonia.Views;

public partial class MainWindow : Window
{
    private readonly bool startup;

    public MainWindow(bool startup)
    {
        InitializeComponent();
        this.startup = startup;
    }
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Config.AppConfig.Instance.Save();
        base.OnClosing(e);
    }

    private void Window_Loaded(object sender,RoutedEventArgs e)
    {
        if (startup)
        {
            Hide();
        }
    }
}
