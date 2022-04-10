using FormTools.FormDescriptor;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class DrawingFieldView : View, IElementView
    {
        

        public DrawingFieldView(DrawingFieldDescriptor descriptor) : base()
        {
            this.Descriptor = descriptor;
            if (descriptor.Mandatory)
            {
                this.Mandatory = true;
            }
            else
            {
                this.BackgroundColor = Color.White;
            }
            this.Strokes = new List<Stroke>();
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.VerticalOptions = LayoutOptions.FillAndExpand;
            this.HeightMultiplier = this.Descriptor.Height / this.Descriptor.Width;
            var tapGesture = new TapGestureRecognizer();

			tapGesture.Tapped += this.Tapped;

            
            this.GestureRecognizers.Add(tapGesture);
            CanLoadDrawingPage = true;
        }

		public async void Tapped(object sender, EventArgs eventArgs)
		{
            if (CanLoadDrawingPage)
            {
                CanLoadDrawingPage = false;
                DependencyService.Get<IOrientation>().SetLandscape();
                await App.Current.MainPage.Navigation.PushModalAsync(new DrawingFieldEntryPage(this), true);
            }
		}

        #region Structs

        public struct Stroke
        {
            public List<Point> Points { get; set; }

            public Stroke(Point startPoint)
            {
                this.Points = new List<Point>();
                this.Points.Add(startPoint);
            }
        }

        public struct Point
        {
            public double X { get; private set; }
            public double Y { get; private set; }

            public Point(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        #endregion

        public void UpdateStrokes()
        {
            this.InvalidateMeasure();
            RequiresUpdate?.Invoke(this, new EventArgs());
        }

        public void DataChanged ()
        {
            this.FieldValueChanged?.Invoke(this, new EventArgs());
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var newSize = new SizeRequest(new Size(widthConstraint, widthConstraint * this.HeightMultiplier));
            this.SizeDifference = widthConstraint / this.Descriptor.Width;
            RequiresUpdate?.Invoke(this, new EventArgs());
            return newSize;
        }
        
        public event RequiresUpdateEventHandler RequiresUpdate;
        public event FieldValueChangedEventHandler FieldValueChanged;

        public double HeightMultiplier { get; set; }
        public double SizeDifference { get; set; }
        public List<Stroke> Strokes { get; set; }

        public DrawingFieldDescriptor Descriptor { get; private set; }

        public ElementDescriptor RawDescriptor { get { return Descriptor; } }

        public bool CanLoadDrawingPage { get; set; }

        public string FieldValue
        {
            get
            {
                return this.Strokes.Count == 0 ? this.Descriptor.NotTickedValue : this.Descriptor.TickedValue;
            }
            set
            {
                
            }
        }

        public string FieldNotShownValue
        {
            get
            {
                return this.Descriptor.NotTickedValue;
            }
        }

        public bool Tickable
        {
            get
            {
                return true;
            }
        }

        public bool Ticked
        {
            get
            {
                return this.Strokes.Count > 0;
            }
        }

        public string FieldValValue
        {
            get
            {
                return null;
            }
        }

        public string PrepopValue
        {
            set
            {
                // not needed
            }
        }

        private bool isMandatory = false;
        public bool Mandatory
        {
            get
            {
                return this.isMandatory;
            }

            set
            {
                this.isMandatory = value;
                this.BackgroundColor = CoreAppTools.MandatoryRed;
            }
        }
    }

    public delegate void RequiresUpdateEventHandler(object sender, EventArgs eventArgs);
}
