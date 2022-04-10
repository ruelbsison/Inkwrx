using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using CoreGraphics;
using System.Linq;
using Xamarin.Forms.Platform.iOS;

namespace INKWRX_Mobile.iOS.UI
{
    public class DrawingView : UIView
    {
        public DrawingView(DrawingFieldView dfv)
        {
            this.DrawingFieldView = dfv;
            var col = this.DrawingFieldView.Descriptor.StrokeColour;
            this.Layer.BorderColor = col.ToColor().ToCGColor();
            this.BackgroundColor = dfv.Descriptor.Mandatory ? CoreAppTools.MandatoryRed.ToUIColor() : UIColor.White;
            this.Layer.BorderWidth = 1;
            this.Layer.CornerRadius = 0;
			var tap = new UITapGestureRecognizer(() =>
			{
				dfv.Tapped(this, new EventArgs());
			});
			this.AddGestureRecognizer(tap);
        }

        public DrawingFieldView DrawingFieldView { get; private set; }


        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            var gctx = UIGraphics.GetCurrentContext();

            gctx.SetStrokeColor(UIColor.Black.CGColor);
            gctx.SetLineWidth(1);
            foreach (var stroke in this.DrawingFieldView.Strokes)
            {
                var path = new CGPath();
                path.AddLines(stroke.Points.Select(x => new CGPoint(x.X * DrawingFieldView.SizeDifference, x.Y * DrawingFieldView.SizeDifference)).ToArray());
                gctx.AddPath(path);
                //TODO: Can this be moved outside the loop?
                gctx.DrawPath(CGPathDrawingMode.Stroke);
            }
            
        }
        
        public void RedrawLines()
        {
            this.SetNeedsDisplay();
        }
    }
}
