using FrpGUI.Util;
using FzLib;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FrpGUI.Config
{
    public abstract class FrpConfigBase : IToFrpConfig, ICloneable
    {
        private bool autoStart;
        private string name;
        private ProcessStatus processStatus = ProcessStatus.NotRun;

        public FrpConfigBase()
        {
            Process = new ProcessHelper(this);
            Process.Exited += Process_Exited;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler StatusChanged;

        public bool AutoStart
        {
            get => autoStart;
            set => this.SetValueAndNotify(ref autoStart, value, nameof(AutoStart));
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public string Name
        {
            get => name;
            set => this.SetValueAndNotify(ref name, value, nameof(Name));
        }

        [JsonIgnore]
        public ProcessHelper Process { get; protected set; }

        [JsonIgnore]
        public ProcessStatus ProcessStatus
        {
            get => processStatus;
            private set => this.SetValueAndNotify(ref processStatus, value, nameof(ProcessStatus));
        }

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
            newItem.processStatus = ProcessStatus.NotRun;
            newItem.Process = new ProcessHelper(this);
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

        public abstract string ToIni();

        public abstract string ToToml();

        private void Process_Exited(object sender, EventArgs e)
        {
            ChangeStatus(ProcessStatus.NotRun);
        }
    }
}