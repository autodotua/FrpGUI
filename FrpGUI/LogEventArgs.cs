namespace FrpGUI
{
    public class LogEventArgs(string message, string instanceName, char type, bool fromFrp) : EventArgs
    {
        public DateTime Time { get; } = DateTime.Now;
        public string Message { get; } = message;
        public string InstanceName { get; } = instanceName;
        public char Type { get; init; } = type;
        public bool FromFrp { get; } = fromFrp;
    }
}