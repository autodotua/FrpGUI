using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.ViewModels;

public partial class FrpStatusInfo : ObservableObject, IFrpProcess
{
    [ObservableProperty]
    private FrpConfigBase config;

    [ObservableProperty]
    private ProcessStatus processStatus;
    public FrpStatusInfo()
    {

    }

    public FrpStatusInfo(FrpConfigBase config)
    {
        this.config = config;
    }

    public FrpStatusInfo(IFrpProcess fp)
    {
        config = fp.Config;
        ProcessStatus = fp.ProcessStatus;
    }

    public event EventHandler StatusChanged;

    public Task RestartAsync()
    {
        throw new NotImplementedException();
    }

    public Task StartAsync()
    {
        throw new NotImplementedException();
    }

    public Task StopAsync()
    {
        throw new NotImplementedException();
    }
}
