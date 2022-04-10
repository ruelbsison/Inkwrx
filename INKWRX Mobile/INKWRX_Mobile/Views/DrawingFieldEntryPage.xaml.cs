using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace INKWRX_Mobile.Views
{
    public partial class DrawingFieldEntryPage : ContentPage
    {
        private DrawingFieldView drawingFieldView = null;
        private DrawingFieldEntryView drawingFieldEntry = null;
        public DrawingFieldEntryPage()
        {
            InitializeComponent();
            this.BackgroundColor = CoreAppTools.LightSilver;
        }

        public DrawingFieldEntryPage(DrawingFieldView dfv) : this()
        {
            this.drawingFieldView = dfv;
            drawingFieldEntry = new DrawingFieldEntryView(this.drawingFieldView);
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.OkButton.HeightRequest = 50;
            this.OkButton.WidthRequest = 50;
            this.CancelButton.HeightRequest = 50;
            this.CancelButton.WidthRequest = 50;
            this.ClearButton.HeightRequest = 50;
            this.ClearButton.WidthRequest = 50;
            this.UndoButton.HeightRequest = 50;
            this.UndoButton.WidthRequest = 50;
            this.OkButton.VerticalOptions = LayoutOptions.CenterAndExpand;
            this.OkButton.HorizontalOptions = LayoutOptions.CenterAndExpand;
            this.CancelButton.VerticalOptions = LayoutOptions.CenterAndExpand;
            this.CancelButton.HorizontalOptions = LayoutOptions.CenterAndExpand;
            this.UndoButton.VerticalOptions = LayoutOptions.CenterAndExpand;
            this.UndoButton.HorizontalOptions = LayoutOptions.CenterAndExpand;
            this.ClearButton.VerticalOptions = LayoutOptions.CenterAndExpand;
            this.ClearButton.HorizontalOptions = LayoutOptions.CenterAndExpand;
            this.OkButton.Aspect = Aspect.AspectFit;
            this.CancelButton.Aspect = Aspect.AspectFit;
            this.ClearButton.Aspect = Aspect.AspectFit;
            this.UndoButton.Aspect = Aspect.AspectFit;
            this.ButtonStack.WidthRequest = 100;

            this.OkButton.Source = CoreAppTools.GetImageSource("Icons/SignaturePopoutScreen/iw_app_ios_icon_ok.png");
            this.CancelButton.Source = CoreAppTools.GetImageSource("Icons/SignaturePopoutScreen/iw_app_ios_icon_cancel.png");
            this.UndoButton.Source = CoreAppTools.GetImageSource("Icons/SignaturePopoutScreen/iw_app_ios_icon_undo.png");
            this.ClearButton.Source = CoreAppTools.GetImageSource("Icons/SignaturePopoutScreen/iw_app_ios_icon_erase.png");

            this.FieldFrame.Content = drawingFieldEntry;
            this.FieldFrame.HorizontalOptions = LayoutOptions.FillAndExpand;
            drawingFieldView.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.FieldFrame.VerticalOptions = LayoutOptions.CenterAndExpand;
            this.drawingFieldView.VerticalOptions = LayoutOptions.CenterAndExpand;
            var cancelTap = new TapGestureRecognizer();
            var okTap = new TapGestureRecognizer();
            var clearTap = new TapGestureRecognizer();
            var undoTap = new TapGestureRecognizer();
            cancelTap.Tapped += async (sender, eventArgs) =>
            {
                await App.Current.MainPage.Navigation.PopModalAsync(true);
                DependencyService.Get<Dependencies.IOrientation>().SetPortrait();
            };

            undoTap.Tapped += (sender, eventArgs) =>
            {
                if (drawingFieldEntry.NewStrokes.Any())
                {
                    drawingFieldEntry.NewStrokes.RemoveAt(drawingFieldEntry.NewStrokes.Count - 1);
                }
                this.drawingFieldEntry.UpdateStrokes();
            };

            okTap.Tapped += async (sender, eventArgs) =>
            {
                this.drawingFieldView.Strokes.Clear();
                this.drawingFieldView.Strokes.AddRange(this.drawingFieldEntry.NewStrokes);
                await App.Current.MainPage.Navigation.PopModalAsync(true);
                DependencyService.Get<Dependencies.IOrientation>().SetPortrait();
                this.drawingFieldView.UpdateStrokes();
                this.drawingFieldView.DataChanged();
            };

            clearTap.Tapped += (sender, eventArgs) =>
            {
                this.drawingFieldEntry.NewStrokes.Clear();
                this.drawingFieldEntry.UpdateStrokes();
            };

            this.OkButton.GestureRecognizers.Add(okTap);
            this.CancelButton.GestureRecognizers.Add(cancelTap);
            this.UndoButton.GestureRecognizers.Add(undoTap);
            this.ClearButton.GestureRecognizers.Add(clearTap);
        }

        protected override void OnDisappearing()
        {
            this.drawingFieldView.CanLoadDrawingPage = true;
        }
    }
}
