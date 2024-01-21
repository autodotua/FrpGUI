﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrpGUI.Avalonia.ViewModels
{
    public partial class SettingWindowViewModel : ViewModelBase
    {

        public bool Startup
        {
        get
            {
                if (OperatingSystem.IsWindows())
                {
                    return Utils.Startup.IsRegistryKeyExist();
                }
                return false;
            }
            set
            {
                OnPropertyChanging(nameof(Startup));
                if (OperatingSystem.IsWindows())
                {
                    if(value)
                    {
                        Utils.Startup.CreateRegistryKey("s");
                    }
                    else
                    {
                        Utils.Startup.DeleteRegistryKey();
                    }
                    OnPropertyChanged(nameof(Startup));
                }
                else
                {
                    throw new PlatformNotSupportedException("仅支持Windows");
                }
            }
        }
    }
}
