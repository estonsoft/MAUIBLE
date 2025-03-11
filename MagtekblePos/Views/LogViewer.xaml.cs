using MagtekblePos.ViewModels;

namespace MagtekblePos.Views;

public partial class LogViewer : ContentPage
{
	private readonly LogViewModel logViewModel = new();

	public LogViewer ()
	{
        InitializeComponent();
        BindingContext = logViewModel;
    }
}