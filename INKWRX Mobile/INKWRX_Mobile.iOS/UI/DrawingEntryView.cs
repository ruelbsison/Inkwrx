using CoreGraphics;
using Foundation;
using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using static INKWRX_Mobile.UI.DrawingFieldView;

namespace INKWRX_Mobile.iOS.UI
{
    public class DrawingEntryView : UIView
    {
        public DrawingEntryView(DrawingFieldEntryView dfv)
        {
            this.DrawingFieldEntryView = dfv;
            var col = this.DrawingFieldEntryView.DrawingFieldView.Descriptor.StrokeColour;
            this.Layer.BorderColor = new CGColor((float)col.Red / 255f, (float)col.Green / 255f, (float)col.Blue / 255f, (float)col.Alpha / 255f);
            this.BackgroundColor = UIColor.White;
            this.Layer.BorderWidth = 1;
            this.Layer.CornerRadius = 0;
            this.drawer = new UILongPressGestureRecognizer();
            this.drawer.AddTarget(this, new ObjCRuntime.Selector("LongPressHandler"));
            this.drawer.MinimumPressDuration = 0;
            this.AddGestureRecognizer(drawer);
        }

        private bool userInBox = false;

        [Export("LongPressHandler")]
        public void LongPressHandler()
        {
            var point = this.drawer.LocationOfTouch(0, this);
            var newPoint = new Point(point.X / this.DrawingFieldEntryView.SizeDifference, point.Y / this.DrawingFieldEntryView.SizeDifference);
            switch (this.drawer.State)
            {
                case UIGestureRecognizerState.Began:
                    this.DrawingFieldEntryView.NewStrokes.Add(new Stroke(newPoint));
                    userInBox = true;
                    break;
                case UIGestureRecognizerState.Changed:
                    if (newPoint.X >= 0 && newPoint.X <= this.DrawingFieldEntryView.DrawingFieldView.Descriptor.Width &&
                        newPoint.Y >= 0 && newPoint.Y <= this.DrawingFieldEntryView.DrawingFieldView.Descriptor.Height)
                    {
                        if (userInBox)
                        {
                            this.DrawingFieldEntryView.NewStrokes[this.DrawingFieldEntryView.NewStrokes.Count - 1].Points.Add(newPoint);
                        }
                        else
                        {
                            goto case UIGestureRecognizerState.Began;
                        }
                    }
                    else
                    {
                        userInBox = false;
                    }
                    break;
            }
            this.RedrawLines();
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            var gctx = UIGraphics.GetCurrentContext();

            gctx.SetStrokeColor(UIColor.Black.CGColor);
            gctx.SetLineWidth(1);

            foreach (var stroke in this.DrawingFieldEntryView.NewStrokes)
            {
                var path = new CGPath();
                path.AddLines(stroke.Points.Select(x => new CGPoint(x.X * DrawingFieldEntryView.SizeDifference, x.Y * DrawingFieldEntryView.SizeDifference)).ToArray());
                //path.CloseSubpath();
                gctx.AddPath(path);
                //TODO: Can this be moved outside the loop?
                gctx.DrawPath(CGPathDrawingMode.Stroke);
            }

        }

        public void RedrawLines()
        {
            this.SetNeedsDisplay();
        }

        public DrawingFieldEntryView DrawingFieldEntryView { get; private set; }


        private UILongPressGestureRecognizer drawer = null;
    }
}
