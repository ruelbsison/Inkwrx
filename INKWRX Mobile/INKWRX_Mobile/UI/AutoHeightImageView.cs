using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class AutoHeightImageView : Image
    {
        public AutoHeightImageView (double aspectDifference) : base()
        {
            this.AspectDifference = aspectDifference;
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var newSize = new SizeRequest(new Size(widthConstraint, widthConstraint * this.AspectDifference));
            
            return newSize;
        }

        public double AspectDifference { get; set; }
    }
}
