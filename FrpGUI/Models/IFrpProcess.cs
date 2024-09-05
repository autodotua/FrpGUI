using FrpGUI.Configs;
using FrpGUI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
