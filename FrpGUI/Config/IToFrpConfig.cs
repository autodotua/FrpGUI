using System.ComponentModel;

namespace FrpGUI.Config
{
    public interface IToFrpConfig : INotifyPropertyChanged
    {
        public string ToIni();

        public string ToToml();
    }
}