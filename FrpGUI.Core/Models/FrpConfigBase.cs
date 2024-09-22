using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace FrpGUI.Models
{
    [JsonDerivedType(typeof(ClientConfig))]
    [JsonDerivedType(typeof(ServerConfig))]
    public abstract partial class FrpConfigBase : ObservableObject, IToFrpConfig, ICloneable
    {
        [ObservableProperty]
        private bool autoStart;

        [ObservableProperty]
        private string dashBoardPassword = "admin";

        [ObservableProperty]
        private ushort dashBoardPort = 7500;

        [ObservableProperty]
        private string dashBoardUsername = "admin";

        [ObservableProperty]
        private string dashBoardAddress = "localhost";

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string token = "";
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