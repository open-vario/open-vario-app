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
	public partial class Header : Grid
    {
		public Header ()
		{
			InitializeComponent ();
            ImageSource src = img.Source;
            src = null;
		}
	}
}