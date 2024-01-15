using Avalonia.Controls;

namespace FrpGUI.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Config.AppConfig.Instance.Save();
        base.OnClosing(e);
    }
}
