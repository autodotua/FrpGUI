using FrpGUI.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FrpGUI.Configs
{
    public abstract class AppConfigBase
    {
        public abstract string ConfigPath { get; }

        protected abstract JsonSerializerContext JsonSerializerContext { get; }

        public static T Get<T>() where T : AppConfigBase, new()
        {
            T config = new T();

            if (OperatingSystem.IsBrowser()
                || File.Exists(config.ConfigPath))
            {
                try
                {
                    config = config.GetImpl<T>();
                }
                catch (Exception ex)
                {
                    config = new T();
                }
            }
            config.OnLoaded();
            return config;
        }

        protected virtual T GetImpl<T>() where T : AppConfigBase
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllBytes(ConfigPath),
                            JsonHelper.GetJsonOptions(JsonSerializerContext));
        }

        public virtual void Save()
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(this, GetType(), JsonHelper.GetJsonOptions(JsonSerializerContext));
            File.WriteAllBytes(ConfigPath, bytes);
        }

        protected virtual void OnLoaded()
        {
        }
    }
}