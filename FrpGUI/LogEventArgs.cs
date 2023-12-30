namespace FrpGUI
{
    public class LogEventArgs(string message, char type) : EventArgs
    {
        public string Message { get; init; } = message;
        public char Type { get; init; } = type;
    }
}