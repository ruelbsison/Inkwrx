using INKWRX_Mobile.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace INKWRX_Mobile.UWP.UI
{
    public class DrawingView : UserControl
    {

        public DrawingView(DrawingFieldView dfv)
        {
            this.drawingFieldView = dfv;
            var b = new Border();
            b.BorderThickness = new Windows.UI.Xaml.Thickness(1);
            b.CornerRadius = new Windows.UI.Xaml.CornerRadius(0);
            b.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb((byte)dfv.Descriptor.StrokeColour.Alpha, (byte)dfv.Descriptor.StrokeColour.Red, (byte)dfv.Descriptor.StrokeColour.Green, (byte)dfv.Descriptor.StrokeColour.Blue));
            b.Child = drawingCanvas;
            b.Padding = new Windows.UI.Xaml.Thickness(0);
            drawingCanvas.Background = dfv.Descriptor.Mandatory ? new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 
                    (byte)(CoreAppTools.MandatoryRed.R * 255), 
                    (byte)(CoreAppTools.MandatoryRed.G * 255),
                    (byte)(CoreAppTools.MandatoryRed.B * 255)))
                 : new SolidColorBrush(Windows.UI.Colors.White);
            this.Content = b;
        }

        private DrawingFieldView drawingFieldView = null;
        
        public void RedrawLines()
        {
            this.drawingCanvas.Children.Clear();
            foreach(var stroke in this.drawingFieldView.Strokes)
            {
                for (var i = 1; i < stroke.Points.Count; i++)
                {
                    var startPoint = stroke.Points[i - 1];
                    var endPoint = stroke.Points[i];
                    var line = new Line
                    {
                        X1 = startPoint.X * drawingFieldView.SizeDifference,
                        Y1 = startPoint.Y * drawingFieldView.SizeDifference,
                        X2 = endPoint.X * drawingFieldView.SizeDifference,
                        Y2 = endPoint.Y * drawingFieldView.SizeDifference,
                        Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                        StrokeThickness = 1
                    };
                    this.drawingCanvas.Children.Add(line);
                }
            }
        }

        private Canvas drawingCanvas = new Canvas();
    }
}
