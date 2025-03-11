using MagtekblePos.ViewModels;
namespace MagtekblePos.Views;

public partial class BLEScanner : ContentPage
{
    private readonly BLEScannerViewModel _viewModel;
    public BLEScanner()
    {
        InitializeComponent();
        _viewModel = new();
        BindingContext = _viewModel;

    }

}