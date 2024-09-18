using FrpGUI.Avalonia.ViewModels;

using FzLib.Avalonia.Dialogs;

namespace FrpGUI.Avalonia.Views;

public partial class SettingsDialog : DialogHost
{
    public SettingsDialog(SettingViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }

    protected override void OnCloseButtonClick()
    {
        Close();
    }
}