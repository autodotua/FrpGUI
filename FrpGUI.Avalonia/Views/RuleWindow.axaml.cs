using Avalonia.Controls;
using Avalonia.Interactivity;
using FrpGUI.Avalonia.ViewModels;

namespace FrpGUI.Avalonia.Views;

public partial class RuleWindow : Window
{
    public RuleWindow()
    {
        DataContext = new RuleWindowViewModel();
        InitializeComponent();
    }
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as RuleWindowViewModel;
        if (vm.Check())
        {
            Close(vm.Rule);
        }
    }
}
