using System.Text.Json;
using System.Text.Json.Serialization;

namespace FrpGUI.Configs
{
    public abstract class AppConfigBase<T> where T : AppConfigBase<T>, new()
    {
        public abstract string ConfigPath { get; }

        protected abstract JsonSerializerContext JsonSerializerContext { get; }

        public static T Get()
        {
            T config = new T();

            if (File.Exists(Path.Combine(AppContext.BaseDirectory, config.ConfigPath)))
            {
                try
                {
                    config = JsonSerializer.Deserialize<T>(File.ReadAllBytes(config.ConfigPath),
                        JsonHelper.GetJsonOptions(AppConfigSourceGenerationContext.Default));
                }
                catch (Exception ex)
                {
                    config = new T();
                }
            }
            config.OnLoaded();
            return config;
        }

        public virtual void Save()
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(this, typeof(T), JsonHelper.GetJsonOptions(JsonSerializerContext));
            File.WriteAllBytes(ConfigPath, bytes);
        }

        protected virtual void OnLoaded()
        {

        }

        public void DoAndSave(Action<T> action)
        {
            action(this as T);
            Save();
        }
    }
}
