using FzLib.DataStorage.Serialization;
using System.Collections.ObjectModel;
using System.IO;

namespace FrpGUI
{
    public class Config : JsonSerializationBase
    {
        private static Config instance;

        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = OpenOrCreate<Config>();
                }
                if (instance.Client == null)
                {
                    instance.Client = new ClientConfig();
                }
                if (instance.Server == null)
                {
                    instance.Server = new ServerConfig();
                }
                return instance;
            }
        }

        public ClientConfig Client { get; set; }
        public ServerConfig Server { get; set; }
        public bool ClientOn { get; set; }
        public bool ServerOn { get; set; }
    }
}