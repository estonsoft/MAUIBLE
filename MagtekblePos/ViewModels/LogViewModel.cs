using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MagtekblePos.ViewModels;

public class LogViewModel
{
    public System.Windows.Input.ICommand ClearLogMessages { get; init; } = new Command(ClearMessages);

    public static ReadOnlyObservableCollection<string> Messages => App.Logger.Messages;

    private static void ClearMessages()
    {
        App.Logger.ClearMessages();
    }
}
