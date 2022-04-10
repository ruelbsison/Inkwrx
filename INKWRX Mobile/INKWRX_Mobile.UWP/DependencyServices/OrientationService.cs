using INKWRX_Mobile.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(INKWRX_Mobile.UWP.DependencyServices.OrientationService))]
namespace INKWRX_Mobile.UWP.DependencyServices
{
    public class OrientationService : IOrientation
    {
        public void SetLandscape()
        {
            var window = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            window.TryResizeView(new Windows.Foundation.Size(window.VisibleBounds.Height, window.VisibleBounds.Width));
        }

        public void SetPortrait()
        {
            var window = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            window.TryResizeView(new Windows.Foundation.Size(window.VisibleBounds.Height, window.VisibleBounds.Width));
        }
    }
}
