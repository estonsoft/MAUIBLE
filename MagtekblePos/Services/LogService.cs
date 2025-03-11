using System.Collections.ObjectModel;

namespace MagtekblePos.Services;
/// <summary>
/// Runs everything on MainThread
/// </summary>
public class LogService
{
    private readonly ObservableCollection<string> LogMessages = [];
    public ReadOnlyObservableCollection<string> Messages { get; init; }

    public LogService()
    {
        Messages = new(LogMessages);
    }

    public void ClearMessages()
    {
        MainThread.BeginInvokeOnMainThread(LogMessages.Clear);
    }

    public void AddMessage(string message)
    {
        MainThread.BeginInvokeOnMainThread(() => { LogMessages.Add(message); });
    }
}
