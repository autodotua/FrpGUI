using FrpGUI.Config;
using FzLib;
using FzLib.WPF;
using Microsoft.Win32;
using ModernWpf.FzExtension.CommonDialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonDialog = ModernWpf.FzExtension.CommonDialog.CommonDialog;

namespace FrpGUI.WPF
{
    public abstract class PanelBase : UserControl, INotifyPropertyChanged
    {
        protected abstract Button StartButton { get; }
        protected abstract Button StopButton { get; }
        protected abstract Button RestartButton { get; }
        protected abstract Button CheckButton { get; }
        protected abstract Button ExportButton { get; }
        protected abstract Control ConfigView { get; }
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
            Resources.Add("ConfigWidth", double.NaN);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            StartButton.Click += StartButton_Click;
            RestartButton.Click += RestartButton_Click;
            StopButton.Click += StopButton_Click;
            CheckButton.Click += CheckButton_Click;
            ExportButton.Click += ExportButton_Click;
            SizeChanged += PanelBase_SizeChanged;
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            string config;
            string filter;
            switch (AppConfig.Instance.FrpConfigType)
            {
                case "INI":
                    config = FrpConfig.ToIni();
                    filter = "INI配置文件|*.ini";
                    break;
                case "TOML":
                    config = FrpConfig.ToToml();
                    filter = "TOML配置文件|*.toml";
                    break;
                default:
                    throw new Exception("未知FRP配置文件类型");
            }
            if (await CommonDialog.ShowYesNoDialogAsync("配置文件", "是否导出配置文件", config))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = filter;
                dialog.FileName = FrpConfig.Name;
                dialog.AddExtension = true;
                if (dialog.ShowDialog() == true)
                {
                    File.WriteAllText(dialog.FileName, config, new UTF8Encoding(false));
                }
            }
        }

        private void PanelBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Resources.Contains("ConfigWidth"))
            {
                Resources["ConfigWidth"] = ConfigView.ActualWidth switch
                {
                    < 500 => ConfigView.ActualWidth - 24,
                    _ => 240d
                };
            }
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
                if (await CommonDialog.ShowYesNoDialogAsync("检查正在运行的进程", $"存在{processes.Length}个frp{FrpConfig.Type}进程，是否停止？"))
                {
                    await FrpConfig.Process.KillExistedProcesses(FrpConfig.Type);
                }
            }
            else
            {
                await CommonDialog.ShowOkDialogAsync("检查正在运行的进程", $"没有正在运行的frp{FrpConfig.Type}进程");
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
            try
            {
                FrpConfig.Start();
            }
            catch (Exception ex)
            {
                CommonDialog.ShowErrorDialogAsync(ex, "frp启动失败");
            }
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