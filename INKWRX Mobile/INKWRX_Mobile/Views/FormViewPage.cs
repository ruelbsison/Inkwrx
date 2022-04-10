using FormTools.FormDescriptor;
using INKWRX_Mobile.Connect.Types;
using INKWRX_Mobile.Database;
using INKWRX_Mobile.Database.Entity;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Util;
using INKWRXPhotoTools_Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using static INKWRX_Mobile.UI.DrawingFieldView;
using static INKWRXPhotoTools_Mobile.PhotoTools;

namespace INKWRX_Mobile.Views
{
    public class FormViewPage : InkwrxBasePage
    {
        public FormViewPage(InkwrxBasePage parentPage, Form form, Transaction original = null, PrepopForm prepop = null) : base("Form", "Backgrounds/FormScreen/iw_app_ios_background_form.png", parentPage)
        {
            this.CurrentForm = form;
            this.Processor = new FormProcessor(((App)App.Current).LoggedInUser, form, DateTime.Now, prepop);
            this.Processor.AttachedImages = new List<ImageEntry>();
            this.ScrollY = 0;
            if (original != null)
            {
                if (original.Status == (int)DatabaseHelper.Status.Sent)
                {
                    //create copy
                    this.Processor.OriginalTransaction = original;

                }
                else if (original.Status == (int)DatabaseHelper.Status.Autosaved)
                {
                    //load autosaved
                    this.Processor.AutosavedTransaction = original;
                    this.discardAutosave = true;
                }
                else
                {
                    this.Processor.OriginalTransaction = original;
                    this.Processor.CurrentTransaction = original;
                }
            }

            this.Scroller = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical
                
            };
            this.ProgressLayout = new AbsoluteLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = CoreAppTools.LightSilver
            };

            var progressStack = new StackLayout
            {
                Padding = 0,
                Orientation = StackOrientation.Vertical
            };

            this.ProgressLabel = new Label
            {
                Text = "Loading Form - Please Wait"
            };

            progressStack.Children.Add(this.ProgressLabel);
            this.ProgressBar = new ActivityIndicator
            {
                HeightRequest = 50,
                WidthRequest = 50,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                IsRunning = true
            };
            progressStack.Children.Add(this.ProgressBar);

            AbsoluteLayout.SetLayoutFlags(progressStack, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(progressStack, new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            this.ProgressLayout.Children.Add(progressStack);


            this.FormContent = new StackLayout
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                Padding = 10,
                Spacing = 20
            };
            this.Scroller.Content = this.FormContent;
            this.BackButton.IsVisible = true;
            
            this.PageContent.Content = this.ProgressLayout;
            
            this.SendButton = new Image
            {
                WidthRequest = 40,
                HeightRequest = 40,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Aspect = Aspect.AspectFit,
                Source = CoreAppTools.GetImageSource("Icons/FormScreen/ToolBar/iw_app_ios_toolbar_icon_send.png")
            };
            this.CameraButton = new Image
            {
                WidthRequest = 40,
                HeightRequest = 40,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Aspect = Aspect.AspectFit,
                Source = CoreAppTools.GetImageSource("Icons/FormScreen/ToolBar/iw_app_ios_toolbar_icon_camera.png")
            };
            this.AttachButton = new Image
            {
                WidthRequest = 40,
                HeightRequest = 40,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Aspect = Aspect.AspectFit,
                Source = CoreAppTools.GetImageSource("Icons/FormScreen/ToolBar/iw_app_ios_toolbar_icon_gallery.png")
            };
            this.ParkButton = new Image
            {
                WidthRequest = 40,
                HeightRequest = 40,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Aspect = Aspect.AspectFit,
                Source = CoreAppTools.GetImageSource("Icons/FormScreen/ToolBar/iw_app_ios_toolbar_icon_prepop.png")
            };

            this.ClearButton = new Image
            {
                WidthRequest = 25,
                HeightRequest = 25,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Aspect = Aspect.AspectFit,
                Source = CoreAppTools.GetImageSource("Icons/FormScreen/NavBar/iw_app_ios_icon_remove.png")
            };

            var parkTap = new TapGestureRecognizer();
            var sendTap = new TapGestureRecognizer();
            var attachTap = new TapGestureRecognizer();
            var cameraTap = new TapGestureRecognizer();
            var clearTap = new TapGestureRecognizer();

            

            parkTap.Tapped += (sender, eventArgs) =>
            {
                if (this.disableButtons || this.PageContent.Content == this.ProgressLayout)
                {
                    return;
                }
                this.disableButtons = true;
                this.ScrollY = this.Scroller.ScrollY;
                this.ProgressLabel.Text = "Parking Form - Please Wait...";
                this.PageContent.Content = this.ProgressLayout;
                this.ProgressBar.IsRunning = true;

                Task.Run(async () =>
                {
                    this.discardAutosave = false;
                    await this.Processor.Park(false);
                    await this.Processor.ResignAutosave();
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        this.Processor.StartAutosaveTimer();

                        this.ProgressBar.IsRunning = false;
                        this.PageContent.Content = this.Scroller;
                        await this.Scroller.ScrollToAsync(0, this.ScrollY, false);
                        this.renderer.DataChanged = false;
                        this.disableButtons = false;
                    });
                });
            };

            sendTap.Tapped += (sender, eventArgs) =>
            {
                if (this.disableButtons || this.PageContent.Content == this.ProgressLayout)
                {
                    return;
                }
                this.disableButtons = true;
                this.ProgressLabel.Text = "Preparing To Send - Please Wait...";
                this.PageContent.Content = this.ProgressLayout;
                this.ProgressBar.IsRunning = true;

                Task.Run(async () =>
                {
                    if (!this.AllFields.Any(field => (field.Tickable && field.Ticked) || field.FieldValue != field.FieldNotShownValue))
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await this.DisplayAlert("Empty Form", "Form not sent - Blank form being sent. Please enter data and Send the form", "Ok");
                            this.PageContent.Content = this.Scroller;
                            this.disableButtons = false;
                        });
                        return;
                    }

                    var ok = await this.Processor.Park(true);
                    if (ok)
                    {
                        this.renderer.DataChanged = false;
                        this.discardAutosave = false;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.PageContent.Content = this.Scroller;
                            this.BackButtonTapped(this, new EventArgs());
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await this.DisplayAlert("Mandatory Fields Required", "Please complete all mandatory fields before sending", "Ok");
                            this.PageContent.Content = this.Scroller;
                            this.disableButtons = false;
                        });
                    }
                });
            };

            attachTap.Tapped += (sender, eventArgs) =>
            {
                if (this.disableButtons || this.PageContent.Content == this.ProgressLayout)
                {
                    return;
                }
                this.disableButtons = true;
                this.ScrollY = this.Scroller.ScrollY;
                App.Current.MainPage = new GalleryAttachPage(this, this.Processor);
                this.disableButtons = false;
            };

            cameraTap.Tapped += async (sender, eventArgs) =>
            {
                if (this.disableButtons || this.PageContent.Content == this.ProgressLayout)
                {
                    return;
                }
                this.disableButtons = true;
                this.ScrollY = this.Scroller.ScrollY;
                await App.Current.MainPage.Navigation.PushModalAsync(new CameraPage(
                    null, 
                    this.Processor.CurrentTransaction == null 
                        ? "-1" 
                        : this.Processor.CurrentTransaction.Id.ToString(),
                    this.Processor));
                this.disableButtons = false;
            };

            clearTap.Tapped += async (sender, eventArgs) =>
            {
                if (this.disableButtons || this.PageContent.Content == this.ProgressLayout)
                {
                    return;
                }
                this.disableButtons = true;

                if (null != this.Processor.CurrentTransaction && this.Processor.CurrentTransaction.Id != -1)
                {
                    var response = await this.DisplayAlert("Delete Form?", "This will delete your parked form. Are you sure?", "Yes", "No");
                    if (!response)
                    {
                        this.disableButtons = false;
                        return;
                    }

                    this.ProgressLabel.Text = "Deleting Parked Form - Please Wait...";
                    this.ProgressBar.IsRunning = true;
                    this.PageContent.Content = this.ProgressLayout;
                    if (await App.DatabaseHelper.DeleteTransactionAsync(this.Processor.CurrentTransaction))
                    {
                        this.Processor.CurrentTransaction = null;
                        this.Processor.ResignAutosave();
                        App.Current.MainPage = this.ParentPage;
                    }
                    else
                    {
                        DependencyService.Get<IToastNotification>().SendToast("Error", "Error deleting parked form");
                        this.PageContent.Content = this.Scroller;
                        this.disableButtons = false;
                    }
                }
                else
                {
                    var response = await this.DisplayAlert("Clear Form?", "This will clear all unsaved data. Are you sure?", "Yes", "No");
                    if (!response)
                    {
                        this.disableButtons = false;
                        return;
                    }
                    this.ProgressLabel.Text = "Clearing Form - Please Wait...";
                    this.ProgressBar.IsRunning = true;
                    this.PageContent.Content = this.ProgressLayout;
                    this.Processor.ResignAutosave();
                    App.Current.MainPage = new FormViewPage(parentPage, this.CurrentForm, null, this.Processor.PrepopForm);
                }
            };

            this.ParkButton.GestureRecognizers.Add(parkTap);
            this.SendButton.GestureRecognizers.Add(sendTap);
            this.AttachButton.GestureRecognizers.Add(attachTap);
            this.CameraButton.GestureRecognizers.Add(cameraTap);
            this.ClearButton.GestureRecognizers.Add(clearTap);

            this.RightButtons.Children.Add(ClearButton);

            this.ToolbarStack.Children.Add(this.SendButton);
            this.ToolbarStack.Children.Add(this.CameraButton);
            this.ToolbarStack.Children.Add(this.AttachButton);
            this.ToolbarStack.Children.Add(this.ParkButton);
            this.ToolbarStack.IsVisible = true;
        }

        protected override void OnDisappearing()
        {
            this.ScrollY = this.Scroller.ScrollY;
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (this.renderer == null)
            {
                Task.Run(() =>
                {
                    this.RenderForm();
                    this.PageLoading = false;
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Device.StartTimer(TimeSpan.FromMilliseconds(100d), () =>
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await this.Scroller.ScrollToAsync(0, this.ScrollY, false);
                            this.PageLoading = false;
                        });
                        return false;
                    });
                });
            }
        }

        public async void RenderForm(bool wait = false)
        {
            try
            {
                //TODO: Switch these when we don't need the test any more
                //var formText = await ReadFileAsync();
                if (wait)
                {
                    await Task.Delay(100);
                }
                var formText = await DependencyService.Get<IFormFileTools>().GetFormData(this.CurrentForm.FormIdentifier, ((App)App.Current).LoggedInUser.Username);

                this.AllFields = new List<IElementView>();
                this.AllPanels = new List<HeaderStackLayout>();

                var form = new FormDescriptor(formText, long.Parse(this.CurrentForm.FormIdentifier));
                this.renderer = new FormRenderer(form, this.FormContent, ((App)App.Current).LoggedInUser);
                this.renderer.FormRenderComplete += async (sender, eventArgs) =>
                {
                    if (this.Processor.AutosavedTransaction != null || this.Processor.OriginalTransaction != null)
                    {
                        if (this.Processor.AutosavedTransaction != null && this.Processor.AutosavedTransaction.PrepopId != -1)
                        {
                            this.Processor.PrepopForm = (await App.DatabaseHelper.GetPrepopForms(this.CurrentForm))
                                .FirstOrDefault(ppf => ppf.Id == this.Processor.AutosavedTransaction.PrepopId);
                        }
                        if (this.Processor.OriginalTransaction != null
                            && this.Processor.OriginalTransaction.Status != (int)DatabaseHelper.Status.Sent
                            && this.Processor.OriginalTransaction.PrepopId != -1)
                        {
                            this.Processor.PrepopForm = (await App.DatabaseHelper.GetPrepopForms(this.CurrentForm))
                                .FirstOrDefault(ppf => ppf.Id == this.Processor.OriginalTransaction.PrepopId);
                        }
                        await this.LoadFormData();
                    }
                    else
                    {
                        this.Processor.StartDate = DateTime.Now;
                        if (this.Processor.PrepopForm != null)
                        {
                            await this.LoadPrepopData();
                        }
                    }

                    foreach (var panel in this.AllPanels)
                    {
                        var tap = new TapGestureRecognizer();
                        tap.Tapped += (tapsender, tapeventArgs) =>
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                foreach (var testPanel in this.AllPanels)
                                {
                                    if (testPanel.Equals(panel))
                                    {

                                        testPanel.PanelShown = !testPanel.PanelShown;

                                    }
                                    else
                                    {

                                        testPanel.PanelShown = false;
                                    }
                                }
                                if (panel.PanelShown)
                                {

                                    await Task.Delay(100);

                                    var viewY = panel.Y + ((StackLayout)panel.Parent).Y;
                                    var viewHeight = panel.Height;
                                    var viewBottom = viewY + viewHeight;
                                    var currentScroll = this.Scroller.ScrollY;
                                    var currentScrollBottom = currentScroll + Scroller.Height;
                                    if (viewY > currentScroll && viewBottom < currentScrollBottom)
                                    {
                                        return;
                                    }
                                    if (viewHeight < this.Scroller.Height)
                                    {
                                        viewY = viewY - ((this.Scroller.Height - viewHeight) / 2);
                                    }

                                    await this.Scroller.ScrollToAsync(0, viewY, true);

                                }
                            });
                        };

                        panel.HeaderFrame.GestureRecognizers.Add(tap);
                    }

                    this.PageContent.Content = this.Scroller;

                    this.Processor.AllFields = this.AllFields;

                    this.Processor.StartAutosaveTimer();
                    this.disableButtons = false;
                    await Task.Delay(100);
                    this.renderer.DataChanged = false;
                };

                this.renderer.FieldAdded += (eventArgs) =>
                {
                    this.AllFields.Add(eventArgs.Element);
                };

                this.renderer.PanelAdded += (eventArgs) =>
                {
                    this.AllPanels.Add(eventArgs.HeaderPanel);
                };

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.renderer.RenderForm();
                });
            }
            catch (Exception)
            {
                var svc = App.WebService.GetServiceCenterClient();
                var user = ((App)App.Current).LoggedInUser;
                var getRequest = new GetZipFormSecure(user.Username, user.Password, int.Parse(this.CurrentForm.FormIdentifier));
                var encDate = Crypto.GetFormattedDate(DateTime.Now);
                svc.SendDataCompleted += (sender, eventArgs) =>
                {
                    if (eventArgs.Error != null)
                    {
                        return;
                    }
                    if (eventArgs.Cancelled)
                    {

                        return;
                    }

                    var decrypted = Crypto.Decrypt(eventArgs.Data, eventArgs.Result);
                    var response = new ResponseItem(XElement.Parse(decrypted));
                    if (response.ErrorCode == 101)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ((App)App.Current).Logout();
                        });
                        return;
                    }
                    if (response.ErrorCode == 0)
                    {
                        var bytes = Convert.FromBase64String(response.ByteData);
                        DependencyService.Get<IFormFileTools>().SaveAndUnzipFormFiles(this.CurrentForm.FormIdentifier, user.Username, bytes);
                        this.RenderForm(true);
                    }
                };
                WebRequest request = null;
                App.TestWebAccess(svc.Endpoint.Address.Uri.ToString(), ref request, (result) =>
                {
                    WebResponse response = null;
                    try
                    {
                        response = request.EndGetResponse(result);

                        svc.SendDataAsync(encDate, Crypto.Encrypt(getRequest.ToXml().ToString(), encDate));
                    }
                    catch
                    {
                        this.RenderForm(true);

                        return;
                    }
                    finally
                    {
                        if (response != null)
                        {
                            response.Dispose();
                        }
                    }
                });
            }
        }

        private async Task LoadPrepopData()
        {
            this.PrepopFields = await App.DatabaseHelper.GetPrepopFields(this.Processor.PrepopForm);
			var activated = new List<string>();
			Calculations.GetInstance().HoldCalculations = true;
            foreach (var field in this.PrepopFields)
            {
                var view = this.AllFields.FirstOrDefault(x => x.RawDescriptor.FdtFieldName == field.FieldName);
                if (view != null)
                {
					if (view is ISOFieldView || view is DecimalFieldView)
					{
						activated.Add(field.FieldName);
					}
                    view.PrepopValue = field.FieldValue;
                }
            }
			Calculations.GetInstance().HoldCalculations = false;
			if (activated.Any() && this.Processor.CurrentTransaction == null)
			{
				Calculations.GetInstance().Recalculate(activated,
						   this.renderer.FormDescriptor.PageDescriptors[0].PageCalcFields,
						   this.FormContent.Children.OfType<HeaderStackLayout>());
			}
        }

        protected override async void GoBack()
        {
            if (this.PageContent.Content == this.ProgressLayout || this.PageLoading)
            {
                return;
            }

            bool okToExit = true;
            if (this.renderer.DataChanged || this.discardAutosave)
            {
                // data changed...
                string message;
                if (this.discardAutosave)
                {
                    message = "Are you sure you wish to discard this Auto-Saved form?";
                }
                else
                {
                    message = "Data on this form has not been saved.\nAre you sure you want to exit?";
                }
                okToExit = await this.DisplayAlert("Unsaved Data", message, "Yes", "No");
            }

            if (okToExit)
            {
                if (!((App)App.Current).LoggingOut)
                {
                    this.Processor.ResignAutosave();
                    DependencyService.Get<IPhotoFileSystem>().ClearNoTransactionFolder();
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    base.GoBack();
                });
            }
        }

        private async Task LoadFormData()
        {
            var fields = new List<Field>();
            var strokes = new List<StrokePath>();
            var strokePoints = new Dictionary<StrokePath, List<StrokePoint>>();
            var images = new List<AttachedItem>();
            if (this.Processor.AutosavedTransaction == null)
            {
                fields = await App.DatabaseHelper.GetFieldsAsync(this.Processor.OriginalTransaction);
                strokes = await App.DatabaseHelper.GetStrokePathsAsync(this.Processor.OriginalTransaction);
                foreach (var stroke in strokes)
                {
                    strokePoints.Add(stroke, await App.DatabaseHelper.GetStrokePointsAsync(stroke));
                }
                images = await App.DatabaseHelper.GetAttachedItemsAsync(this.Processor.OriginalTransaction);
            }
            else
            {
                this.Processor.OriginalTransaction = await App.DatabaseHelper.GetTransactionAsync(this.Processor.AutosavedTransaction.AutosavedParent);
                this.Processor.CurrentTransaction = this.Processor.OriginalTransaction;

                fields = await App.DatabaseHelper.GetFieldsAsync(this.Processor.AutosavedTransaction);
                strokes = await App.DatabaseHelper.GetStrokePathsAsync(this.Processor.AutosavedTransaction);
                foreach (var stroke in strokes)
                {
                    strokePoints.Add(stroke, await App.DatabaseHelper.GetStrokePointsAsync(stroke));
                }
                images = await App.DatabaseHelper.GetAttachedItemsAsync(this.Processor.AutosavedTransaction);
            }

            if (this.Processor.OriginalTransaction?.Status == (int)DatabaseHelper.Status.Sent)
            {

                this.Processor.CurrentTransaction = new Transaction
                {
                    StartedDate = DateTime.Now,
                    SentDate = null,
                    SavedDate = null,
                    Status = (int)DatabaseHelper.Status.Available,
                    AutosavedDateTime = null,
                    AutosavedParent = -1,
                    FormName = this.Processor.OriginalTransaction.FormName,
                    Form = this.Processor.OriginalTransaction.Form,
                    PrepopId = -1,
                    User = this.Processor.OriginalTransaction.User,
                    OriginalAddedDate = this.Processor.OriginalTransaction.StartedDate
                };
            }

            if (this.Processor.CurrentTransaction != null)
            {
                this.Processor.StartDate = this.Processor.CurrentTransaction.StartedDate;
            }
            else if (this.Processor.AutosavedTransaction != null)
            {
                this.Processor.StartDate = this.Processor.AutosavedTransaction.StartedDate;
            }
            this.Processor.AttachedImages = new List<ImageEntry>();
            foreach (var image in images)
            {
                this.Processor.AttachedImages.Add(new ImageEntry
                {
                    ImageReference = image.Reference,
                    ImageType = image.ItemSource == (int)DatabaseHelper.AttachmentSource.Gallery
                        ? ImageEntry.ImageEntryType.Gallery
                        : ImageEntry.ImageEntryType.Camera,
                    Orientation = image.ItemSource == (int)DatabaseHelper.AttachmentSource.Gallery ? -1 : 0
                });
            }

			var addedFields = new List<string>();
            Calculations.GetInstance().HoldCalculations = true;
            foreach (var field in fields)
            {
                var view = this.AllFields.FirstOrDefault(v => v.RawDescriptor.FieldId == field.Name);
                if (view != null)
                {
                    if (view is TabletImageView)
                    {
                        if (!string.IsNullOrEmpty(field.ShownValue))
                        {
                            var entry = this.Processor.AttachedImages.FirstOrDefault(i => i.ImageReference == field.ShownValue);

                            ((TabletImageView)view).AttachedImage = entry;
                        }
                        continue;
                    }
                    if (view is DrawingFieldView)
                    {
                        var dfield = (DrawingFieldView)view;
                        var viewStrokes = strokes.Where(s => s.FieldName == dfield.Descriptor.FieldId).ToList();
                        dfield.Strokes = viewStrokes.Select(stroke => new Stroke
                            {
                                Points = strokePoints[stroke].Select(x => new DrawingFieldView.Point(x.X, x.Y)).ToList()
                            }).ToList();
                        dfield.UpdateStrokes();
                        continue;
                    }
                    if (view is TickBoxFieldView)
                    {
                        if (field.Ticked)
                        {
                            ((TickBoxFieldView)view).IsChecked = true;
                        }
                        continue;
                    }
                    if (view is RadioButtonFieldView)
                    {
                        if (field.Ticked)
                        {
                            ((RadioButtonFieldView)view).IsOn = true;
                        }
                        continue;
                    }
                    if (view is DropDownFieldView || view is DateTimeFieldView || view is ISOFieldView  || view is DecimalFieldView)
                    {
                        if (!string.IsNullOrEmpty(field.ShownValue))
                        {
                            view.FieldValue = field.ShownValue;
							if (view is ISOFieldView || view is DecimalFieldView)
							{
								addedFields.Add(view.RawDescriptor.FdtFieldName);
							}
                            if (view is DecimalFieldView)
                            {
                                ((DecimalFieldView)view).OldText = field.ShownValue;
                            }
                        }

                        continue;
                    }
                    if (view is NotesFieldView)
                    {
                        if (!string.IsNullOrEmpty(field.ShownValue))
                        {
                            view.FieldValue = field.ShownValue;
                        }
                    }
                }
            }

            if (this.Processor.PrepopForm != null)
            {
                await this.LoadPrepopData();
            }
            else
            {
                Calculations.GetInstance().HoldCalculations = false;
				await Task.Delay(100); // Allow any previous calculations to finish
                Calculations.GetInstance().Recalculate(
                addedFields,
                this.renderer.FormDescriptor.PageDescriptors[0].PageCalcFields,
                this.FormContent.Children.OfType<HeaderStackLayout>());
                // not needed - parked data should be correct.
            }
        }

        /// <summary>
        /// Reads the file containing form details.
        /// </summary>
        /// <returns>Returns the contents of the form data file.</returns>
        public async Task<string> ReadFileAsync()
        {
            
            var assembly = typeof(FormViewPage).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("INKWRX_Mobile.mobile_svg_v2.svg");
            var text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = await reader.ReadToEndAsync();
            }

            return text;

        }

        public ScrollView Scroller { get; set; }
        public StackLayout FormContent { get; set; }
        public AbsoluteLayout ProgressLayout { get; set; }
        public ActivityIndicator ProgressBar { get; set; }
        public Label ProgressLabel { get; set; }

        public FormProcessor Processor { get; set; }

        private List<IElementView> AllFields { get; set; }
        private List<HeaderStackLayout> AllPanels { get; set; }
        

        //public Transaction OriginalTransaction { get; set; }
        //public Transaction CurrentTransaction { get; set; }
        //public Transaction AutosavedTransaction { get; set; }

        public List<Field> CurrentFields { get; set; }
        public List<StrokePath> CurrentStrokes { get; set; }

        private FormRenderer renderer = null;

        
        public List<PrepopField> PrepopFields { get; set; }

        public Form CurrentForm { get; set; }
        public Image SendButton { get; set; }
        public Image CameraButton { get; set; }
        public Image AttachButton { get; set; }
        public Image ParkButton { get; set; }
        public Image ClearButton { get; set; }
        public double ScrollY { get; set; }
        private bool discardAutosave = false;
        private bool disableButtons = true;
    }
}
