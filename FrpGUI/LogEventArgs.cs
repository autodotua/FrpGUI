namespace FrpGUI
{
    public class LogEventArgs(string message, string instanceName, char type, bool fromFrp, Exception exception) : EventArgs
    {
        public DateTime Time { get; } = DateTime.Now;
        public string Message { get; } = message;
        public string InstanceName { get; } = instanceName;
        public char Type { get; } = type;
        public bool FromFrp { get; } = fromFrp;
        public Exception Exception { get; } = exception;
    }
}