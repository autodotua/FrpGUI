using FrpGUI.Configs;
using FrpGUI.Enums;
using FrpGUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.Models;

public partial class FrpProcess : IFrpProcess, INotifyPropertyChanged
{
    private ProcessStatus processStatus;

    public FrpProcess()
    {

    }
    public FrpProcess(FrpConfigBase config)
    {
        Config = config;
        ProcessStatus = ProcessStatus.Stopped;
    }
    public FrpConfigBase Config { get; set; }
    public ProcessStatus ProcessStatus
    {
        get => processStatus; set
        {
            if (processStatus != value)
            {
                processStatus = value;
                StatusChanged?.Invoke(this, EventArgs.Empty);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessStatus)));
            }
        }
    }

    public event EventHandler StatusChanged;
    public event PropertyChangedEventHandler PropertyChanged;

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


[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(FrpProcess))]
[JsonSerializable(typeof(AppConfig))]
[JsonSerializable(typeof(LogEntity))]
[JsonSerializable(typeof(List<LogEntity>))]
[JsonSerializable(typeof(IList<FrpProcess>))]
internal partial class FrpAvaloniaSourceGenerationContext : JsonSerializerContext
{
}
