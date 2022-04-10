using INKWRX_Mobile.Connect.Types;
using INKWRX_Mobile.Database;
using INKWRX_Mobile.Database.Entity;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.Util;
using INKWRX_Mobile.Views;
using INKWRXPhotoTools_Mobile;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using static INKWRXPhotoTools_Mobile.PhotoTools;

namespace INKWRX_Mobile.Connect
{
    public class FormSendingService
    {
        public FormSendingService(User user)
        {
            this.User = user;
        }

        public void StartSendingService()
        {
            this.cancel = false;
            this.SendOnce();
        }

        private void SendOnce(bool delay = false)
        {
            if (cancel)
            {
                return;
            }
            Device.BeginInvokeOnMainThread(() =>
			{
                if (Device.OnPlatform(true, false, false))
				{
					DependencyService.Get<IBackgroundService>().RegisterBackgroundTask();
				}
                Device.StartTimer(TimeSpan.FromSeconds(delay ? 30 : 15), this.SendRound);
				
            });
            
        }

        public void CancelSending()
        {
            this.cancel = true;
        }

        private bool SendRound ()
        {
            if (cancel)
            {
                return false;
            }

            this.SendRoundAsync();

            return false;
        }

        private async void SendRoundAsync()
        {
            if (cancel)
            {
                return;
            }

            var nextForUser = await App.DatabaseHelper.GetTransactionsAsync(this.User, Database.DatabaseHelper.Status.Pending);
            if (!nextForUser.Any())
            {
                
				if (Device.OnPlatform(true, false, false))
				{
					DependencyService.Get<IBackgroundService>().EndBackgroundTask();
				}
				this.SendOnce(true);
                return;
            }
            this.currentSendingTransaction = nextForUser.First();
            this.currentAttachedItems = await App.DatabaseHelper.GetAttachedItemsAsync(this.currentSendingTransaction);
            this.form = await App.DatabaseHelper.GetFormAsync(this.currentSendingTransaction.Form);
            this.filesSent = new Dictionary<string, DateTime>();
            this.newFileNames = new Dictionary<string, string>();
            this.currentAttachedIndex = 0;
            this.fields = await App.DatabaseHelper.GetFieldsAsync(this.currentSendingTransaction);
            this.strokes = await App.DatabaseHelper.GetStrokePathsAsync(this.currentSendingTransaction);
			this.strokePoints = new Dictionary<StrokePath, List<StrokePoint>>();
            foreach (var stroke in strokes)
            {
                var points = await App.DatabaseHelper.GetStrokePointsAsync(stroke);
                this.strokePoints.Add(stroke, points);
            }

            if (currentAttachedItems.Any())
            {
                
                this.SendNextPhoto();
                return;
            }

            this.SendTransactionInfo();
        }

        private async void SendNextPhoto()
        {
            var image = this.currentAttachedItems[currentAttachedIndex];
            
            var filename = string.Format("{0}_{1}_{2}_{3}_{4}.jpg", 
                image.ItemSource == (int)DatabaseHelper.AttachmentSource.Gallery ? "gallery" : "camera",
                this.User.Username,
                "mobile",
                this.currentSendingTransaction.Id,
                this.currentAttachedIndex);
            this.newFileNames.Add(image.Reference, filename);
            if (image.Status == (int)DatabaseHelper.Status.Sent)
            {
                this.filesSent.Add(filename, image.SentDateTime.Value);
                this.currentAttachedIndex++;
                if (this.currentAttachedIndex > this.currentAttachedItems.Count - 1)
                {
                    this.SendTransactionInfo();
                }
                else
                {
                    this.SendNextPhoto();
                }
                return;
            }

            var entry = new ImageEntry
            {
                ImageReference = image.Reference,
                ImageType = image.ItemSource == (int)DatabaseHelper.AttachmentSource.Gallery ? ImageEntry.ImageEntryType.Gallery : ImageEntry.ImageEntryType.Camera,
                Orientation = image.ItemSource == (int)DatabaseHelper.AttachmentSource.Gallery ? -1 : 0
            };

            fileData = null;
            fileData = await DependencyService.Get<IPhotoFileSystem>().GetImage(entry);

            if (Device.OnPlatform(true, false, false))
            {
                fileData = await DependencyService.Get<IImageResizer>().ResizeImage(fileData, 4d);
            } 

            var maxPackets = (int)Math.Ceiling((double)fileData.Length / (double)ChunkSize);
            this.startSendDesc = new StartSendFilePacketWithTablet(User.Username, User.Password, filename, maxPackets, ChunkSize, fileData.Length);
            var encDate = Crypto.GetFormattedDate(DateTime.Now);
            var svc = App.WebService.GetFormServiceClient();
            svc.SendDataCompleted += (sender, eventArgs) =>
            {
                if (eventArgs.Error != null)
                {
                    
                    this.SendOnce(true);
                    return;
                }
                if (eventArgs.Cancelled)
                {
                    return;
                }
                var decrypted = Crypto.Decrypt(eventArgs.Data, eventArgs.Result);
                var destResp = new ResponseItem(XElement.Parse(decrypted));
                if (destResp.ErrorCode == 101)
                {
                    this.CancelSending();
                    ((App)App.Current).Logout();
                    return;
                }

                if (destResp.ErrorCode == 0)
                {
                    if (destResp.NextPacketId > -1 && destResp.NextPacketId < this.startSendDesc.MaxPackets)
                    {
                        this.nextSendingChunk = destResp.NextPacketId;
                        this.SendNextChunk();
                    }
                    else
                    {
                        this.FinaliseFile();
                    }
                }
                else
                {
                    //error
                    this.formError(destResp.ErrorCode.ToString(), destResp.Message);
                    return;
                }

            };

            WebRequest request = null;
            App.TestWebAccess(svc.Endpoint.Address.Uri.ToString(), ref request, (result) =>
            {
                WebResponse response = null;
                try
                {
                    response = request.EndGetResponse(result);
                    svc.SendDataAsync(encDate, Crypto.Encrypt(startSendDesc.ToXml().ToString(), encDate));
                }
                catch
                {
                    this.formError("No Connection", "Could not connect to server");
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

        private void formError(string code, string message)
        {
            
            DependencyService.Get<IToastNotification>().SendToast("Form Sending Error: " + code, message);
            this.SendOnce(true);
        }

        private void SendNextChunk()
        {
            var length = this.fileData.Length;
            var offset = this.nextSendingChunk * this.ChunkSize;
            var thisChunkSize = length - offset > this.ChunkSize ? this.ChunkSize : length - offset;
            var chunk = new byte[thisChunkSize];
            Array.Copy(this.fileData, offset, chunk, 0, thisChunkSize);
            var b64Chunk = Convert.ToBase64String(chunk);

            var packetData = new SendFilePacketWithTablet(this.User.Username, this.User.Password, b64Chunk, this.nextSendingChunk);
            var svc = App.WebService.GetFormServiceClient();
            var encDate = Crypto.GetFormattedDate(DateTime.Now);
            svc.SendDataCompleted += (sender, eventArgs) =>
            {
                if (eventArgs.Error != null)
                {
                    if (Device.OnPlatform(true, false, false))
                    {
                        DependencyService.Get<IBackgroundService>().EndBackgroundTask();
                    }
                    this.SendOnce(true);
                    return;
                }
                if (eventArgs.Cancelled)
                {
                    return;
                }
                var decrypted = Crypto.Decrypt(eventArgs.Data, eventArgs.Result);
                var destResp = new ResponseItem(XElement.Parse(decrypted));
                if (destResp.ErrorCode == 101)
                {
                    this.CancelSending();
                    ((App)App.Current).Logout();
                    return;
                }
                if (destResp.ErrorCode == 0)
                {
                    if (destResp.NextPacketId > -1 && destResp.NextPacketId < this.startSendDesc.MaxPackets)
                    {
                        this.nextSendingChunk = destResp.NextPacketId;
                        this.SendNextChunk();
                    }
                    else
                    {
                        this.FinaliseFile();
                    }
                }
                else
                {
                    this.formError(destResp.ErrorCode.ToString(), destResp.Message);
                }
                    
            };

            WebRequest request = null;
            App.TestWebAccess(svc.Endpoint.Address.Uri.ToString(), ref request, (result) =>
            {
                WebResponse response = null;
                try
                {
                    response = request.EndGetResponse(result);
                    svc.SendDataAsync(encDate, Crypto.Encrypt(packetData.ToXml().ToString(), encDate));

                }
                catch
                {
                    this.formError("No Connection", "Could not connect to server");
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

        private void FinaliseFile()
        {
            var finishFileSend = new FinishSendFilePacketWithTablet(this.User.Username, this.User.Password, this.startSendDesc.FileName, this.startSendDesc.MaxPackets);
            var svc = App.WebService.GetFormServiceClient();
            var encDate = Crypto.GetFormattedDate(DateTime.Now);
            svc.SendDataCompleted += async (sender, eventArgs) =>
            {
                if (eventArgs.Error != null)
                {
                    
                    this.SendOnce(true);
                    return;
                }
                if (eventArgs.Cancelled)
                {
                    return;
                }
                var decrypted = Crypto.Decrypt(eventArgs.Data, eventArgs.Result);
                var destResp = new ResponseItem(XElement.Parse(decrypted));
                if (destResp.ErrorCode == 101)
                {
                    this.CancelSending();
                    ((App)App.Current).Logout();
                    return;
                }
                if (destResp.ErrorCode == 0)
                {
                    this.filesSent.Add(this.startSendDesc.FileName, DateTime.Now);
                    var photo = this.currentAttachedItems[this.currentAttachedIndex];
                    photo.Status = (int)DatabaseHelper.Status.Sent;
                    photo.SentDateTime = this.filesSent[this.startSendDesc.FileName];
                    await App.DatabaseHelper.UpdateItemAsync(photo);
                    this.currentAttachedIndex++;
                    if (this.currentAttachedIndex > this.currentAttachedItems.Count - 1)
                    {
                        this.SendTransactionInfo();
                    }
                    else
                    {
                        this.SendNextPhoto();
                    }
                }
                else
                {
                    this.formError(destResp.ErrorCode.ToString(), destResp.Message);
                }
            };

            WebRequest request = null;
            App.TestWebAccess(svc.Endpoint.Address.Uri.ToString(), ref request, (result) =>
            {
                WebResponse response = null;
                try
                {
                    response = request.EndGetResponse(result);

                    svc.SendDataAsync(encDate, Crypto.Encrypt(finishFileSend.ToXml().ToString(), encDate));
                }
                catch
                {
                    this.formError("No Connection", "Could not connect to server");

            		
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

        
        private async void SendTransactionInfo()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            Position position;
			if (Device.OnPlatform(true, false, false) && DependencyService.Get<IBackgroundService>().IsInBackground())
			{
				position = ((App)App.Current).CurrentPosition;
			}
			else {
				try
				{
					position = await locator.GetPositionAsync(10000);
				}
				catch (GeolocationException)
				{
					//gps is switched off, so send with empty gps
					position = GetEmptyGps();
                }
                catch (TaskCanceledException)
				{
                    //wifi is switched off, gps is switched on
                    position = GetEmptyGps();
                }
            }
            var prepop = await App.DatabaseHelper.GetPrepopForms(this.form);
            var ppForm = prepop.FirstOrDefault(x => x.Id == this.currentSendingTransaction.PrepopId);
            var ppId = ppForm == null ? -1 : ppForm.Identifier;
            var transXml = GetTransactionXml(this.filesSent, this.currentSendingTransaction.StartedDate, DateTime.Now, position, ppId);
            var strokes = GetStrokeXml();
            var proc = GetProcXml();


            var saveTrans = new SaveEformWithXml(this.User.Username, this.User.Password, 
                this.GetProcXml().ToString(), this.GetStrokeXml().ToString(), 
                int.Parse(this.form.FormIdentifier), "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + transXml.ToString());
			var saveText = saveTrans.ToXml().ToString();

            var encDate = Crypto.GetFormattedDate(DateTime.Now);
            var svc = App.WebService.GetFormServiceClient();
            svc.SendDataCompleted += async (sender, eventArgs) =>
            {
                if (eventArgs.Error != null)
                {
                    
                    this.SendOnce(true);
                    return;
                }
                if (eventArgs.Cancelled)
                {
                    
                    return;
                }
                var decrypted = Crypto.Decrypt(eventArgs.Data, eventArgs.Result);
                var destResp = new ResponseItem(XElement.Parse(decrypted));
                if (destResp.ErrorCode == 101)
                {
                    this.CancelSending();
                    ((App)App.Current).Logout();
                    return;
                }
                if (destResp.ErrorCode == 0)
                {
                    
                    if (this.currentSendingTransaction.PrepopId != -1)
                    {
                        if (ppForm != null)
                        {
                            ppForm.Status = (int)DatabaseHelper.Status.Sent;
                            await App.DatabaseHelper.UpdateItemAsync(ppForm);
                        }
                    }

                    this.currentSendingTransaction.Status = (int)DatabaseHelper.Status.Sent;
                    this.currentSendingTransaction.SentDate = DateTime.Now;
                    await App.DatabaseHelper.UpdateItemAsync(this.currentSendingTransaction);
                    DependencyService.Get<IToastNotification>().SendToast("Form Sending Complete", "Form Sent Successfully");

                    if (App.Current.MainPage is HomePage)
                    {
                        ((HomePage)App.Current.MainPage).RefreshUser();
                    }
                    if (App.Current.MainPage is HistoryPage)
                    {
                        ((HistoryPage)App.Current.MainPage).RefreshHistory();
                    }
                    this.SendOnce();

                }
                else
                {
                    this.formError(destResp.ErrorCode.ToString(), destResp.Message);
                }
            };

            WebRequest request = null;
            App.TestWebAccess(svc.Endpoint.Address.Uri.ToString(), ref request, (result) =>
            {
                WebResponse response = null;
                try
                {
                    response = request.EndGetResponse(result);

                    svc.SendDataAsync(encDate, Crypto.Encrypt(saveTrans.ToXml().ToString(), encDate));
                }
                catch
                {
                    this.formError("No Connection", "Could not connect to server");
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

        private Position GetEmptyGps()
        {
            Position position = new Position();
            position.Latitude = 0;
            position.Longitude = 0;
            position.Altitude = 0;
            position.Timestamp = DateTimeOffset.Now;
            return position;
        }

        private XElement GetStrokeXml()
        {
            var form = EFormXmlBuilder.StrokeForm();
            var subform = EFormXmlBuilder.StrokesSubForm(this.form, this.currentSendingTransaction.StartedDate, DateTime.Now, this.strokes.Count);
            var strokepages = EFormXmlBuilder.StrokesPages();
            var strokepage = EFormXmlBuilder.StrokesPage(1);
            var strokestrokes = EFormXmlBuilder.StrokesStrokes();
            var strokeunassigned = EFormXmlBuilder.StrokesUnassigned();
            var strokesFields = new Dictionary<string, XElement>();
            foreach (var stroke in this.strokes)
            {
                var points = this.strokePoints[stroke];
                var minx = points.Select(point => point.X).Min() + stroke.FieldX;
                var miny = points.Select(point => point.Y).Min() + stroke.FieldY;
                var maxx = points.Select(point => point.X).Max() + stroke.FieldX;
                var maxy = points.Select(point => point.Y).Max() + stroke.FieldY;
                var strokeX = EFormXmlBuilder.StrokeField(stroke.FieldName, minx, miny, maxx, maxy);
                strokeX.Add(points.Select(point => EFormXmlBuilder.StrokeSample(point.X + stroke.FieldX, point.Y + stroke.FieldY)));

                if (!strokesFields.ContainsKey(stroke.FieldName))
                {
                    strokesFields[stroke.FieldName] = EFormXmlBuilder.StrokeFieldContainer(stroke.FieldName);
                }
                strokesFields[stroke.FieldName].Add(strokeX);
            }
            strokestrokes.Add(strokeunassigned);
            foreach (var key in strokesFields.Keys)
            {
                strokestrokes.Add(strokesFields[key]);
            }
            strokepage.Add(strokestrokes);
            strokepages.Add(strokepage);
            subform.Add(strokepages);
            form.Add(subform);
            var testText = form.ToString();
            return form;
            
        }

        private XElement GetProcXml()
        {
            var form = EFormXmlBuilder.ProcForm();
            form.Add(EFormXmlBuilder.ProcFormHeader(this.form, this.currentSendingTransaction.StartedDate, DateTime.Now));
            var subform = EFormXmlBuilder.ProcSubForm();
            var page = EFormXmlBuilder.ProcPage(1);
            var procfields = EFormXmlBuilder.ProcFields();
            foreach (var field in this.fields)
            {
                var fieldVal = field.ShownValue ?? "";
                if (this.newFileNames.ContainsKey(fieldVal))
                {
                    fieldVal = this.newFileNames[fieldVal];
                }
                procfields.Add(EFormXmlBuilder.ProcField(field.Name, fieldVal, field.Tickable, field.Ticked, null));
            }
            page.Add(procfields);
            subform.Add(page);
            form.Add(subform);
            return form;
        }

        private static XDocument GetTransactionXml(Dictionary<string, DateTime> filesSent, DateTime startDate, DateTime sentDate, Position gps, int prepopId)
        {
            var transXml = new XDocument();
            var root = new XElement(TransNS + "dtran");
            var files = new XElement(TransNS + "files");
            foreach (var file in filesSent)
            {
                files.Add(new XElement(TransNS + "att",
                    new XAttribute("filename", file.Key),
                    new XAttribute("type", "img"),
                    new XAttribute("sendtime", file.Value.ToString(DateFormat)))
                );
            }
            var formTran = new XElement(TransNS + "formtran",
                new XAttribute("id", "1"));
            if (prepopId != -1)
            {
                formTran.Add(new XAttribute("prepopkey", prepopId));
            }
            formTran.Add(files);
            var pgc = new XElement(TransNS + "pgc",
                new XAttribute("filename", "filename1"),
                new XAttribute("recvtime", startDate.ToString(DateFormat)),
                new XAttribute("sendtime", sentDate.ToString(DateFormat)));
            var gpsunit = new XElement(TransNS + "gpsloc",
                new XAttribute("acc", "gps"),
                new XAttribute("long", gps.Longitude),
                new XAttribute("lat", gps.Latitude),
                new XAttribute("alt", gps.Altitude),
                new XAttribute("time", gps.Timestamp.DateTime.ToString(DateFormat))
                );
            root.Add(new XElement(TransNS + "form", formTran, pgc, gpsunit));
            transXml.Add(root);
            return transXml;
        }

        private static XNamespace TransNS = @"http://destiny.com/xml/routertrans";

        private byte[] fileData = null;

        private int ChunkSize = 50000;

        private static string DateFormat = "yyyy-MM-dd'T'HH:mm:ss.fff";

        private Form form = null;
        private StartSendFilePacketWithTablet startSendDesc = null;
        private int nextSendingChunk = 0;
        private Dictionary<string, string> newFileNames = new Dictionary<string, string>();
        private Dictionary<string, DateTime> filesSent = new Dictionary<string, DateTime>();
        private int currentAttachedIndex = 0;
        private Transaction currentSendingTransaction = null;
        private List<AttachedItem> currentAttachedItems = null;
        private bool cancel = false;
        private List<Field> fields = null;
        private List<StrokePath> strokes = null;
        private Dictionary<StrokePath, List<StrokePoint>> strokePoints = null;
        public User User { get; set; }
    }
}
