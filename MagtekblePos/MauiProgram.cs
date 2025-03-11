using MagtekblePos.Services;

namespace MagtekblePos
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .Services.AddSingleton<IAlertService, AlertService>();

#if DEBUG
            //builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static bool IsAndroid => DeviceInfo.Current.Platform == DevicePlatform.Android;

        public static bool IsMacCatalyst => DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst;

        public static bool IsMacOS => DeviceInfo.Current.Platform == DevicePlatform.macOS;
    }
}
