using FzLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FrpGUI
{
    public abstract class PanelBase : UserControl, INotifyPropertyChanged
    {
        protected abstract Button StartButton { get; }
        protected abstract Button StopButton { get; }
        protected abstract Button RestartButton { get; }
        protected abstract Button CheckButton { get; }
        private FrpConfigBase frpConfig;

        public event PropertyChangedEventHandler PropertyChanged;

        public FrpConfigBase FrpConfig
        {
            get => frpConfig;
            protected set
            {
                if (frpConfig != null)
                {
                    frpConfig.StatusChanged -= FrpConfig_StatusChanged;
                }
                frpConfig = value;
                if (frpConfig != null)
                {
                    frpConfig.StatusChanged += FrpConfig_StatusChanged;
                }
                this.Notify(nameof(FrpConfig));
                UpdateUI();
            }
        }

        public virtual void SetConfig(FrpConfigBase config)
        {
            FrpConfig = config;
        }

        public PanelBase()
        {
            DataContext = this;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            StartButton.Click += StartButton_Click;
            RestartButton.Click += RestartButton_Click;
            StopButton.Click += StopButton_Click;
            CheckButton.Click += CheckButton_Click;
        }

        private void FrpConfig_StatusChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private async void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var processes = await FrpConfig.Process.GetExistedProcesses(FrpConfig.Type);
            if (processes.Length > 0)
            {
                if (await (Window.GetWindow(this) as MainWindow).ShowYesNoMessage($"存在{processes.Length}个frp{FrpConfig.Type}进程，是否停止？"))
                {
                    await FrpConfig.Process.KillExistedProcesses(FrpConfig.Type);
                }
            }
            else
            {
                await (Window.GetWindow(this) as MainWindow).ShowMessage($"没有正在运行的frp{FrpConfig.Type}进程");
            }
            (sender as Button).IsEnabled = true;
        }

        protected virtual void UpdateUI()
        {
            Dispatcher.Invoke(() =>
            {
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = false;
                RestartButton.IsEnabled = false;
                CheckButton.IsEnabled = false;
                switch (FrpConfig.ProcessStatus)
                {
                    case ProcessStatus.NotRun:
                        StartButton.IsEnabled = true;
                        CheckButton.IsEnabled = true;
                        break;

                    case ProcessStatus.Running:
                        StopButton.IsEnabled = true;
                        RestartButton.IsEnabled = true;
                        break;

                    case ProcessStatus.Busy:
                        break;

                    default:
                        break;
                }
            });
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }

        public virtual void Start()
        {
            FrpConfig.Start();
        }

        public virtual Task StopAsync()
        {
            return FrpConfig.StopAsync();
        }

        private async void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            await FrpConfig.RestartAsync();
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            await StopAsync();
        }
    }
}