using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OpenVario
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SplashScreen : ContentPage
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="duration">Display duration in milliseconds</param>
        /// <param name="next_page">Next page to display</param>
        public SplashScreen (uint duration, ContentPage next_page)
		{
			InitializeComponent ();

            Device.StartTimer(TimeSpan.FromMilliseconds(duration), () => { Application.Current.MainPage = next_page; return false; });
		}
	}
}