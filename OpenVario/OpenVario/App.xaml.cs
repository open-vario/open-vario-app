using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OpenVario
{
    public partial class App : Application
    {
        public IPlatform PlatformDesc { get; set; }

        public App(IPlatform platform)
        {
            InitializeComponent();

            PlatformDesc = platform;
            MainPage = new SplashScreen(1500, new MainPage(PlatformDesc.BleStack));
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
