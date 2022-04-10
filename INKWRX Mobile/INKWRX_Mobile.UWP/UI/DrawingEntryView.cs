using INKWRX_Mobile.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace INKWRX_Mobile.UWP.UI
{
    public class DrawingEntryView : UserControl
    {

        public DrawingEntryView(DrawingFieldEntryView dfv)
        {
            this.drawingFieldEntryView = dfv;
            var b = new Border();
            b.BorderThickness = new Windows.UI.Xaml.Thickness(1);
            b.CornerRadius = new Windows.UI.Xaml.CornerRadius(0);
            b.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb((byte)dfv.DrawingFieldView.Descriptor.StrokeColour.Alpha, (byte)dfv.DrawingFieldView.Descriptor.StrokeColour.Red, (byte)dfv.DrawingFieldView.Descriptor.StrokeColour.Green, (byte)dfv.DrawingFieldView.Descriptor.StrokeColour.Blue));
            b.Child = drawingCanvas;
            b.Padding = new Windows.UI.Xaml.Thickness(0);
            drawingCanvas.Background = new SolidColorBrush(Windows.UI.Colors.White);
            this.Content = b;
        }

        private DrawingFieldEntryView drawingFieldEntryView = null;
        
        public void RedrawLines()
        {
            this.drawingCanvas.Children.Clear();
            foreach (var stroke in drawingFieldEntryView.NewStrokes)
            {
                for (var i = 1; i < stroke.Points.Count; i++)
                {
                    var startPoint = stroke.Points[i - 1];
                    var endPoint = stroke.Points[i];
                    var line = new Line
                    {
                        X1 = startPoint.X * drawingFieldEntryView.SizeDifference,
                        Y1 = startPoint.Y * drawingFieldEntryView.SizeDifference,
                        X2 = endPoint.X * drawingFieldEntryView.SizeDifference,
                        Y2 = endPoint.Y * drawingFieldEntryView.SizeDifference,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Windows.UI.Colors.Black)
                       
                    };
                    this.drawingCanvas.Children.Add(line);
                }
            }
        }

        private bool pointerDown = false;

        protected override void OnPointerPressed(PointerRoutedEventArgs eventArgs)
        {
            base.OnPointerPressed(eventArgs);
            pointerDown = true;
            this.drawingFieldEntryView.NewStrokes.Add(new DrawingFieldView.Stroke(new DrawingFieldView.Point(
                    eventArgs.GetCurrentPoint(this).Position.X / this.drawingFieldEntryView.SizeDifference,
                    eventArgs.GetCurrentPoint(this).Position.Y / this.drawingFieldEntryView.SizeDifference
                ))); // create new stroke with this point
            this.RedrawLines();
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs eventArgs)
        {
            base.OnPointerMoved(eventArgs);
            if (pointerDown)
            {
                if (this.drawingFieldEntryView.NewStrokes.Count == 0)
                {
                    this.drawingFieldEntryView.NewStrokes.Add(new DrawingFieldView.Stroke(new DrawingFieldView.Point(
                        eventArgs.GetCurrentPoint(this).Position.X / this.drawingFieldEntryView.SizeDifference,
                        eventArgs.GetCurrentPoint(this).Position.Y / this.drawingFieldEntryView.SizeDifference
                    )));
                }
                var stroke = this.drawingFieldEntryView.NewStrokes[this.drawingFieldEntryView.NewStrokes.Count - 1];
                stroke.Points.Add(new DrawingFieldView.Point(eventArgs.GetCurrentPoint(this).Position.X / this.drawingFieldEntryView.SizeDifference,
                        eventArgs.GetCurrentPoint(this).Position.Y / this.drawingFieldEntryView.SizeDifference));
                this.RedrawLines();
            }
        }

        protected override void OnPointerCanceled(PointerRoutedEventArgs eventArgs)
        {
            base.OnPointerCanceled(eventArgs);
            this.pointerDown = false;
        }

        protected override void OnPointerExited(PointerRoutedEventArgs eventArgs)
        {
            base.OnPointerExited(eventArgs);
            this.pointerDown = false;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs eventArgs)
        {
            base.OnPointerReleased(eventArgs);
            this.pointerDown = false;
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs eventArgs)
        {
            base.OnPointerCaptureLost(eventArgs);
            this.pointerDown = false;
        }

        private Canvas drawingCanvas = new Canvas();
    }
}
