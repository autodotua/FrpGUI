using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using FrpGUI.Avalonia.ViewModels;
using FrpGUI.Avalonia.Views;
using FrpGUI.Config;
using FzLib.Avalonia;
using FzLib.Avalonia.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Views;

public partial class ControlBar : UserControl
{
    private MainViewModel viewModel;

    public ControlBar()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        viewModel = (this.GetVisualAncestors().OfType<MainView>().FirstOrDefault() ?? throw new System.Exception("�Ҳ���MainView"))
            .DataContext as MainViewModel;
    }
}