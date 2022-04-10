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

namespace INKWRX_Mobile.Droid.UI
{
    public class DrawingEntryView : View, View.IOnTouchListener
    {
        public DrawingFieldEntryView DrawingFieldEntryView { get; private set; }

        private Paint BoxPaint;
        private Paint StrokePaint;

        private bool UserInBox;

        public DrawingEntryView(Context context, DrawingFieldEntryView dfv) :
            base(context)
        {
            this.DrawingFieldEntryView = dfv;

            this.SetBackgroundColor(Color.White);

            this.BoxPaint = new Paint();
            this.BoxPaint.Dither = (true);
            this.BoxPaint.SetStyle(Paint.Style.Stroke);
            this.BoxPaint.StrokeJoin = Paint.Join.Round;
            this.BoxPaint.StrokeCap = Paint.Cap.Round;
            this.BoxPaint.StrokeWidth = 1;
            this.BoxPaint.Color = Color.Black;

            var col = this.DrawingFieldEntryView.DrawingFieldView.Descriptor.StrokeColour;
            this.StrokePaint = new Paint();
            this.StrokePaint.Dither = (true);
            this.StrokePaint.SetStyle(Paint.Style.Stroke);
            this.StrokePaint.StrokeJoin = Paint.Join.Round;
            this.StrokePaint.StrokeCap = Paint.Cap.Round;
            this.StrokePaint.StrokeWidth = 1;
            this.StrokePaint.Color = new Color((int)(col.Red / 255f), (int)(col.Green / 255f), (int)(col.Blue / 255f));

            this.SetOnTouchListener(this);
        }

        public bool OnTouch(View view, MotionEvent motionEvent)
        {
            float x = motionEvent.GetX();
            float y = motionEvent.GetY();
            var newPoint = new DrawingFieldView.Point(x / this.DrawingFieldEntryView.SizeDifference, y / this.DrawingFieldEntryView.SizeDifference);

            switch (motionEvent.Action) {
                case MotionEventActions.Down:
                    if (newPoint.X > 0 && newPoint.X < this.DrawingFieldEntryView.DrawingFieldView.Descriptor.Width && newPoint.Y > 0 && newPoint.Y < this.DrawingFieldEntryView.DrawingFieldView.Descriptor.Height)
                    {
                        this.DrawingFieldEntryView.NewStrokes.Add(new DrawingFieldView.Stroke(newPoint));
                        UserInBox = true;
                    }
                    break;
                case MotionEventActions.Move:
                    if(newPoint.X > 0 && newPoint.X < this.DrawingFieldEntryView.DrawingFieldView.Descriptor.Width && newPoint.Y > 0 && newPoint.Y < this.DrawingFieldEntryView.DrawingFieldView.Descriptor.Height)
                    {
                        if (UserInBox)
                        {
                            this.DrawingFieldEntryView.NewStrokes[this.DrawingFieldEntryView.NewStrokes.Count - 1].Points.Add(newPoint);
                            this.RedrawLines();
                        }
                        else
                        {
                            this.DrawingFieldEntryView.NewStrokes.Add(new DrawingFieldView.Stroke(newPoint));
                            UserInBox = true;
                        }
                    }
                    else
                    {
                        UserInBox = false;
                    }
                    break;
            }

            return true;
        }

        public void RedrawLines()
        {
            this.Invalidate();
        }

        [Android.Runtime.Register("onDraw", "(Landroid/graphics/Canvas;)V", "")]
        protected override sealed void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            this.DrawingFieldEntryView.SizeDifference = this.Width / this.DrawingFieldEntryView.DrawingFieldView.Descriptor.Width;

            canvas.DrawRect(0, 0, (int)(this.DrawingFieldEntryView.DrawingFieldView.Descriptor.Width * this.DrawingFieldEntryView.SizeDifference - 1), (int)(this.DrawingFieldEntryView.DrawingFieldView.Descriptor.Height * this.DrawingFieldEntryView.SizeDifference - 1), this.BoxPaint);
            
            List<DrawingFieldView.Stroke> DrawingFieldViewPointList = new List<DrawingFieldView.Stroke>(this.DrawingFieldEntryView.NewStrokes);
            foreach (DrawingFieldView.Stroke stroke in DrawingFieldViewPointList)
            {
                Path path = new Path();
                List<DrawingFieldView.Point> pointList = stroke.Points;
                path.MoveTo((float)(pointList[0].X * this.DrawingFieldEntryView.SizeDifference), (float)(pointList[0].Y * this.DrawingFieldEntryView.SizeDifference));
                int pointCount = pointList.Count;
                for (int pointIterator = 1; pointIterator < pointCount; pointIterator++)
                {
                    path.LineTo((float)(pointList[pointIterator].X * this.DrawingFieldEntryView.SizeDifference), (float)(pointList[pointIterator].Y * this.DrawingFieldEntryView.SizeDifference));
                }
                canvas.DrawPath(path, this.StrokePaint);
            }
        }

        protected override void OnDetachedFromWindow()
        {
            this.DrawingFieldEntryView = null;
            this.BoxPaint = null;
            this.StrokePaint = null;
            this.SetOnTouchListener(null);
        }
    }
}