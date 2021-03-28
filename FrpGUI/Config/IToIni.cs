using System.ComponentModel;

namespace FrpGUI
{
    public interface IToIni : INotifyPropertyChanged
    {
        public string ToIni();
    }
}