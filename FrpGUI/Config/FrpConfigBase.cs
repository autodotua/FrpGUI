using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Util;
using FzLib;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FrpGUI.Config
{
    [JsonDerivedType(typeof(ClientConfig))]
    [JsonDerivedType(typeof(ServerConfig))]
    public abstract partial class FrpConfigBase :ObservableObject, IToFrpConfig, ICloneable
    {
        [ObservableProperty]
        private bool autoStart;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        [property:JsonIgnore]
        private ProcessStatus processStatus = ProcessStatus.NotRun;

        public FrpConfigBase()
        {
            Process = new ProcessHelper(this);
            Process.Exited += Process_Exited;
        }

        public event EventHandler StatusChanged;

        public Guid ID { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public ProcessHelper Process { get; protected set; }

        public abstract char Type { get; }

        public void ChangeStatus(ProcessStatus status)
        {
            Logger.Info("进程状态改变：" + status.ToString(), Name);
            ProcessStatus = status;
            StatusChanged?.Invoke(this, new EventArgs());
        }

        public virtual object Clone()
        {
            var newItem = MemberwiseClone() as FrpConfigBase;
            newItem.ProcessStatus = ProcessStatus.NotRun;
            newItem.Process = new ProcessHelper(newItem);
            return newItem;
        }

        public async Task RestartAsync()
        {
            ChangeStatus(ProcessStatus.Busy); 
            try
            {
                await Process.RestartAsync();
            }
            catch (Exception ex)
            {
                ChangeStatus(ProcessStatus.NotRun);
                throw;
            }
            ChangeStatus(ProcessStatus.Running);
            AppConfig.Instance.Save();
        }

        public void Start()
        {
            ChangeStatus(ProcessStatus.Busy);
            try
            {
                Process.Start(Type, this);
                ChangeStatus(ProcessStatus.Running);
            }
            catch (Exception ex)
            {
                ChangeStatus(ProcessStatus.NotRun);
                throw;
            }
            AppConfig.Instance.Save();
        }

        public async Task StopAsync()
        {
            ChangeStatus(ProcessStatus.Busy);
            await Process.StopAsync();
            ChangeStatus(ProcessStatus.NotRun);
        }

        public abstract string ToToml();

        private void Process_Exited(object sender, EventArgs e)
        {
            ChangeStatus(ProcessStatus.NotRun);
        }
    }
}