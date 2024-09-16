﻿using CommunityToolkit.Mvvm.ComponentModel;
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
    private FrpConfigBase config;

    private ProcessStatus processStatus;

    public FrpStatusInfo()
    {

    }

    public FrpStatusInfo(FrpConfigBase config)
    {
        Config = config;
    }

    public FrpStatusInfo(IFrpProcess fp)
    {
        Config = fp.Config;
        ProcessStatus = fp.ProcessStatus;
    }

    public event EventHandler StatusChanged;

    public FrpConfigBase Config
    {
        get => config;
        set => SetProperty(ref config, value, nameof(Config));
    }
    public ProcessStatus ProcessStatus
    {
        get => processStatus;
        set => SetProperty(ref processStatus, value, nameof(ProcessStatus));
    }
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
