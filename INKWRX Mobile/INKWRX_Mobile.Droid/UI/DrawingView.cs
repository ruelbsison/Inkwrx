using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using INKWRX_Mobile.UI;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;

namespace INKWRX_Mobile.Droid.UI
{
    public class DrawingView : View
    {
        public DrawingFieldView DrawingFieldView { get; private set; }

        private Paint BoxPaint;
        private Paint StrokePaint;

        public DrawingView(Context context, DrawingFieldView dfv):
            base(context)
        {
            this.DrawingFieldView = dfv;

            this.SetBackgroundColor(dfv.Descriptor.Mandatory ? CoreAppTools.MandatoryRed.ToAndroid() : Color.White);

            this.BoxPaint = new Paint();
            this.BoxPaint.Dither = (true);
            this.BoxPaint.SetStyle(Paint.Style.Stroke);
            this.BoxPaint.StrokeJoin = Paint.Join.Round;
            this.BoxPaint.StrokeCap = Paint.Cap.Round;
            this.BoxPaint.StrokeWidth = 1;
            this.BoxPaint.Color = Color.Black;

            var col = this.DrawingFieldView.Descriptor.StrokeColour;
            this.StrokePaint = new Paint();
            this.StrokePaint.Dither = (true);
            this.StrokePaint.SetStyle(Paint.Style.Stroke);
            this.StrokePaint.StrokeJoin = Paint.Join.Round;
            this.StrokePaint.StrokeCap = Paint.Cap.Round;
            this.StrokePaint.StrokeWidth = 1;
            this.StrokePaint.Color = new Color((int)(col.Red / 255f), (int)(col.Green / 255f), (int)(col.Blue / 255f));
        }

        [Android.Runtime.Register("onDraw", "(Landroid/graphics/Canvas;)V", "")]
        protected override sealed void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            this.DrawingFieldView.SizeDifference = this.Width / this.DrawingFieldView.Descriptor.Width;

            canvas.DrawRect(0, 0, (int)(this.DrawingFieldView.Descriptor.Width * this.DrawingFieldView.SizeDifference - 1), (int)(this.DrawingFieldView.Descriptor.Height * this.DrawingFieldView.SizeDifference - 1), this.BoxPaint);

            List<DrawingFieldView.Stroke> DrawingFieldViewPointList = new List<DrawingFieldView.Stroke>(this.DrawingFieldView.Strokes);
            foreach (DrawingFieldView.Stroke stroke in DrawingFieldViewPointList)
            {
                Path path = new Path();
                List<DrawingFieldView.Point> pointList = stroke.Points;
                path.MoveTo((float)(pointList[0].X * this.DrawingFieldView.SizeDifference), (float)(pointList[0].Y * this.DrawingFieldView.SizeDifference));
                int pointCount = pointList.Count;
                for (int pointIterator = 1; pointIterator < pointCount; pointIterator++)
                {
                    path.LineTo((float)(pointList[pointIterator].X * this.DrawingFieldView.SizeDifference), (float)(pointList[pointIterator].Y * this.DrawingFieldView.SizeDifference));
                }
                canvas.DrawPath(path, this.StrokePaint);
            }
        }

        public void RedrawLines()
        {
            this.Invalidate();
        }

        protected override void OnDetachedFromWindow()
        {
            this.DrawingFieldView = null;
            this.BoxPaint = null;
            this.StrokePaint = null;
        }
    }
}