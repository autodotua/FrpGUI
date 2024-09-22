using FrpGUI.Enums;

namespace FrpGUI.Models
{
    public interface IFrpProcess
    {
        public FrpConfigBase Config { get; }

        public ProcessStatus ProcessStatus { get; set; }

        public Task StartAsync();

        public Task StopAsync();

        public Task RestartAsync();

        public event EventHandler StatusChanged;
    }
}