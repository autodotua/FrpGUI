using Avalonia.Controls;
using FrpGUI.Avalonia.ViewModels;

namespace FrpGUI.Avalonia.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        DataContext = new MainViewModel();
        InitializeComponent();
    }
}
