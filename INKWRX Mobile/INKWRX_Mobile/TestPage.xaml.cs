using INKWRX_Mobile.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace INKWRX_Mobile
{
    public partial class TestPage : ContentPage
    {
        public TestPage()
        {
            InitializeComponent();
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //HeaderLabel.Text = DependencyService.Get<IDeviceDetails>().GetDeviceId();
        }

        public StackLayout GetStackView { get { return StackView; } }
    }
}
