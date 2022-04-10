using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class DrawingFieldEntryView : View
    {
        public DrawingFieldEntryView(DrawingFieldView dfv)
        {
            DrawingFieldView = dfv;
            NewStrokes = dfv.Strokes.ToList();
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            //var newSize = new SizeRequest(new Size(this.ParentView.ParentView.Width, heightConstraint));
            var newWidth = this.ParentView.ParentView.Width - 100 - ((Frame)this.ParentView).Padding.HorizontalThickness;
            var newSize = new SizeRequest(new Size(newWidth, newWidth * this.DrawingFieldView.HeightMultiplier));
            this.SizeDifference = newWidth / this.DrawingFieldView.Descriptor.Width;
            this.RequiresUpdate(this, new EventArgs());
            return newSize;
        }
        public void UpdateStrokes()
        {
            RequiresUpdate(this, new EventArgs());
        }
        public List<DrawingFieldView.Stroke> NewStrokes { get; set; }

        public DrawingFieldView DrawingFieldView { get; set; }
        public double SizeDifference { get; set; }
        public event RequiresUpdateEventHandler RequiresUpdate;
    }
}
