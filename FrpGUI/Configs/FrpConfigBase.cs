﻿using CommunityToolkit.Mvvm.ComponentModel;
using FrpGUI.Enums;
using FzLib;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FrpGUI.Configs
{
    [JsonDerivedType(typeof(ClientConfig))]
    [JsonDerivedType(typeof(ServerConfig))]
    public abstract partial class FrpConfigBase : ObservableObject, IToFrpConfig, ICloneable
    {
        [ObservableProperty]
        private bool autoStart;

        [ObservableProperty]
        private string name;

        public FrpConfigBase()
        {
        }

        public string ID { get; set; } = Guid.NewGuid().ToString();

        public abstract char Type { get; }

        public virtual object Clone()
        {
            var newItem = MemberwiseClone() as FrpConfigBase;
            return newItem;
        }

        public abstract string ToToml();

    }
}