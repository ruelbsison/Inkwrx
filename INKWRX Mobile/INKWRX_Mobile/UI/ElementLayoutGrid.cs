using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class ElementLayoutGrid : StackLayout
    {
        public ElementLayoutGrid(List<IElementView> elements)
        {
            this.Elements = elements.OrderBy(x => x.RawDescriptor.Origin.X).ToList();
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.VerticalOptions = LayoutOptions.Start;
            this.ImagesX = this.Elements.Select(x => x.RawDescriptor.Origin.X).Min();
            this.ImagesWidth = this.Elements.Select(x => x.RawDescriptor.Origin.X + x.RawDescriptor.Width).Max() - this.ImagesX;
            this.ImagesY = this.Elements.Select(x => x.RawDescriptor.Origin.Y).Min();
            this.ImagesHeight = this.Elements.Select(x => x.RawDescriptor.Origin.Y + x.RawDescriptor.Height).Max() - this.ImagesY;
            this.Spacing = 0;
            this.Orientation = StackOrientation.Horizontal;
            foreach (var element in this.Elements)
            {
                this.Children.Add((View)element);
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            this.SizeDifference = widthConstraint / this.ImagesWidth;
            var currentX = 0d;
			var heightResized = false;
            foreach (var element in this.Elements)
            {
                ((View)element).Margin = new Thickness((element.RawDescriptor.Origin.X - this.ImagesX - currentX) * this.SizeDifference,
                    (element.RawDescriptor.Origin.Y - this.ImagesY) * this.SizeDifference, 0, 0);

                ((View)element).WidthRequest = element.RawDescriptor.Width * this.SizeDifference;
				if (element is RectangleView && ((StackLayout)((RectangleView)element).Content).Children.Any())
				{
					var childSize = ((RectangleView)element).Content.Measure(((View)element).WidthRequest, double.PositiveInfinity);
					((View)element).HeightRequest = childSize.Request.Height;
					heightResized = true;
					this.ImagesHeight = Math.Max(this.ImagesHeight, element.RawDescriptor.Origin.Y - this.ImagesY + ((View)element).HeightRequest);
				}
				else 
				{
					((View)element).HeightRequest = element.RawDescriptor.Height * this.SizeDifference;
				}
                currentX = element.RawDescriptor.Origin.X + element.RawDescriptor.Width - this.ImagesX;
            }

            return new SizeRequest(new Size(widthConstraint, heightResized ? this.ImagesHeight : this.ImagesHeight * this.SizeDifference));
        }

        public List<IElementView> Elements { get; private set; }
        public double ImagesX { get; private set; }
        public double ImagesWidth { get; private set; }
        public double ImagesY { get; private set; }
        public double ImagesHeight { get; private set; }
        public double SizeDifference { get; private set; }
    }
}
