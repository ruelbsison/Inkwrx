using INKWRX_Mobile.Database;
using INKWRX_Mobile.Database.Entity;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Views;
using INKWRXPhotoTools_Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using static INKWRXPhotoTools_Mobile.PhotoTools;

namespace INKWRX_Mobile.Util
{
    public class FormProcessor
    {
        public FormProcessor(User user, Form form, DateTime startDate, PrepopForm prepopForm)
        {
            this.user = user;
            this.form = form;
            this.StartDate = startDate;
            this.PrepopForm = prepopForm;
        }

        public async Task<bool> Park(bool forSending, bool autosave = false)
        {
            return await Task.Run(async () =>
            {
                if (forSending)
                {
                    foreach (var field in this.AllFields.Where(x => x.Mandatory))
                    {
                        if (field is RadioButtonFieldView ||
                        (field is TickBoxFieldView && ((null != ((TickBoxFieldView)field).Descriptor.GroupName)) && ((TickBoxFieldView)field).Descriptor.GroupName != ""))
                        {
                            continue;
                        }
                        if (field is TickBoxFieldView)
                        {
                            if (! ((TickBoxFieldView)field).Ticked)
                            {
                                return false;
                            }
                            continue;
                        }
                        if (field.FieldValue == field.FieldNotShownValue && (!(field is DrawingFieldView )|| !((DrawingFieldView)field).Strokes.Any()))
                        { 
                            return false;
                        }
                    }
                    var radios = this.AllFields.OfType<RadioButtonFieldView>().Where(x => x.Mandatory).ToList();
                    foreach (var radio in radios)
                    {
                        var radioGroup = radios.Where(x => x.Descriptor.GroupName == radio.Descriptor.GroupName);
                        bool hasValue = false;
                        foreach (var rd in radioGroup)
                        {
                            if (rd.Ticked)
                            {
                                hasValue = true;
                                break;
                            }
                        }
                        if (!hasValue)
                        {
                            return false;
                        }
                    }

                    var ticks = this.AllFields.OfType<TickBoxFieldView>().Where(x => x.Descriptor.GroupName != "").Where(x => x.Mandatory).ToList();
                    foreach (var tick in ticks)
                    {
                        var tickGroup = ticks.Where(x => x.Descriptor.GroupName == tick.Descriptor.GroupName);
                        bool hasValue = false;
                        foreach (var tk in tickGroup)
                        {
                            if (tk.Ticked)
                            {
                                hasValue = true;
                                break;
                            }
                        }
                        if (!hasValue)
                        {
                            return false;
                        }
                    }

                }

                if (autosave)
                {
                    if (this.AutosavedTransaction == null)
                    {
                        this.AutosavedTransaction = new Transaction
                        {
                            Form = this.form.Id,
                            FormName = this.form.FormName,
                            User = this.user.Id,
                            StartedDate = this.StartDate
                        };
                    }

                    if (this.CurrentTransaction != null)
                    {
                        this.AutosavedTransaction.AutosavedParent = this.CurrentTransaction.Id;
                    }
                    this.AutosavedTransaction.AutosavedDateTime = DateTime.Now;

                    await this.SaveTransactionData(this.AutosavedTransaction, false, true);
                }
                else
                {
                    if (this.CurrentTransaction == null)
                    {
                        this.CurrentTransaction = new Transaction
                        {
                            Form = this.form.Id,
                            FormName = this.form.FormName,
                            User = this.user.Id,
                            StartedDate = this.StartDate
                        };
                    }
                    await this.SaveTransactionData(this.CurrentTransaction, forSending, false);
                }

                return true;
            }).ConfigureAwait(true);
            
        }

        public async Task ResignAutosave()
        {
            this.StopAutosaving();
            if (this.AutosavedTransaction != null)
            {
                await App.DatabaseHelper.DeleteTransactionAsync(this.AutosavedTransaction);
                this.AutosavedTransaction = null;
                if (App.Current.MainPage is HistoryPage)
                {
                    ((HistoryPage)App.Current.MainPage).RefreshHistory();
                }
            }
        }

        private async Task<bool> SaveTransactionData(Transaction sourceTransaction, bool forSending, bool autosave)
        {
            sourceTransaction.SavedDate = DateTime.Now;
            if (this.PrepopForm != null)
            {
                sourceTransaction.PrepopId = this.PrepopForm.Id;
            }
            sourceTransaction.Status = autosave 
                ? (int)DatabaseHelper.Status.Autosaved
                : forSending
                    ? (int)DatabaseHelper.Status.Available
                    : (int)DatabaseHelper.Status.Parked;
            if (sourceTransaction.Id == -1)
            {
                await App.DatabaseHelper.AddTransacionAsync(sourceTransaction);
            }
            else
            {
                await App.DatabaseHelper.UpdateItemAsync(sourceTransaction);
                await App.DatabaseHelper.ClearTransactionData(sourceTransaction);
            }
            if (this.PrepopForm != null)
            {
                if (!autosave && this.AutosavedTransaction != null)
                {
                    this.AutosavedTransaction.AutosavedParent = sourceTransaction.Id;
                    await App.DatabaseHelper.UpdateItemAsync(this.AutosavedTransaction);
                }
                this.PrepopForm.Status = forSending
                    ? (int)DatabaseHelper.Status.Pending
					: autosave && this.OriginalTransaction == null 
						? (int)DatabaseHelper.Status.Autosaved 
						: (int)DatabaseHelper.Status.Parked;
                await App.DatabaseHelper.UpdateItemAsync(this.PrepopForm);
            }
            sourceTransaction.FormName = this.form.FormName;

            List<ImageEntry> AttachedImagesCopy = this.AttachedImages.ToList();//prevent ConcurrentModificationException caused by attaching an image while autosave is iterating
            foreach (var entry in AttachedImagesCopy)
			{
				if ((entry.ImageType == ImageEntry.ImageEntryType.Camera
						&& entry.ImageReference.Contains("NoTransaction")) // move no transaction files
					|| (                                                   // or autosaved files in the autosave's directory
						!autosave                                          // into the parked transaction's folder instead
						&& this.AutosavedTransaction != null
						&& entry.ImageType == ImageEntry.ImageEntryType.Camera
						&& entry.ImageReference.Contains(this.AutosavedTransaction.Id.ToString())
					))
				{
					var newEntry = await DependencyService.Get<IPhotoFileSystem>().MoveCameraImage(entry, sourceTransaction.Id.ToString());
					entry.ImageReference = newEntry.ImageReference;
				}

				await App.DatabaseHelper.AddAttachedItemAsync(new AttachedItem
				{
					Transaction = sourceTransaction.Id,
					ItemSource = entry.ImageType == ImageEntry.ImageEntryType.Gallery ? (int)DatabaseHelper.AttachmentSource.Gallery : (int)DatabaseHelper.AttachmentSource.Device,
					ItemType = (int)DatabaseHelper.AttachmentType.Photo,
					Reference = entry.ImageReference,
					Status = autosave
						? (int)DatabaseHelper.Status.Autosaved
						: forSending
							? (int)DatabaseHelper.Status.Pending
							: (int)DatabaseHelper.Status.Parked
				});
			}

            var fields = this.AllFields.Select(view => new Field
            {
                Transaction = sourceTransaction.Id,
                Name = view.RawDescriptor.FieldId,
                ShownValue = view.FieldValue,
                NotShownValue = view.FieldNotShownValue,
                Tickable = view.Tickable,
                Ticked = view.Ticked
            }).ToList();
            await App.DatabaseHelper.AddFieldsAsync(fields);

            

            foreach (var drawingField in this.AllFields.OfType<DrawingFieldView>().ToList())
            {
                foreach (var stroke in drawingField.Strokes)
                {
                    var newStroke = await App.DatabaseHelper.AddStrokePathAsync(new StrokePath
                    {
                        Transaction = sourceTransaction.Id,
                        FieldName = drawingField.Descriptor.FieldId,
                        FieldX = drawingField.Descriptor.Origin.X,
                        FieldY = drawingField.Descriptor.Origin.Y
                    });
                    var points = stroke.Points.Select(point => new StrokePoint
                    {
                        X = point.X,
                        Y = point.Y,
                        Path = newStroke.Id
                    }).ToList();
                    await App.DatabaseHelper.AddStrokePointsAsync(points);

                }
            }

            if (forSending)
            {
                sourceTransaction.Status = (int)DatabaseHelper.Status.Pending;
                await App.DatabaseHelper.UpdateItemAsync(sourceTransaction);
            }
            
            if (!autosave)
            {
                DependencyService.Get<IToastNotification>().SendToast("Form Saved", string.Format("Form {0}", forSending ? "saved for sending" : "parked"));
            }

            return true;

        }

        public async void AutoSave(bool restart = false, bool cancel = false)
        {
            await this.Park(false, true);
            if (cancel)
            {
                this.StopAutosaving();
                return;
            }
            if (restart)
            {
                this.RestartAutosaveTimer();
            }
        }

        public void StopAutosaving()
        {
            
            this.autosaveHandler?.CancelTimer();
            this.autosaveHandler = null;
        }

        private void RestartAutosaveTimer()
        {
            this.autosaveHandler?.CancelTimer();
            this.StartAutosaveTimer();
        }

        public void StartAutosaveTimer()
        {
            this.autosaveHandler = new AutosaveHandler(this);
            this.autosaveHandler.StartTimer();
        }

        private AutosaveHandler autosaveHandler { get; set; }
        
        public PrepopForm PrepopForm { get; set; }
        public DateTime StartDate { get; set; }

        public Transaction CurrentTransaction { get; set; }
        public Transaction AutosavedTransaction { get; set; }
        public Transaction OriginalTransaction { get; set; }
        
        public List<ImageEntry> AttachedImages { get; set; }

        public List<IElementView> AllFields { get; set; }
        private User user;
        private Form form;
    }
}
