using INKWRX_Mobile.UI;
using INKWRX_Mobile.Views.PageModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using INKWRX_Mobile.Database;
using INKWRX_Mobile.Database.Entity;
using INKWRX_Mobile.Connect;
using INKWRX_Mobile.Connect.Types;
using static INKWRX_Mobile.Views.PageModels.HomePageLinkModel;
using INKWRX_Mobile.Util;
using System.Xml.Linq;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.Connect.Prepop;
using System.Net;

namespace INKWRX_Mobile.Views
{
    public class HomePage : InkwrxBasePage
    {
        public HomePage(InkwrxBasePage parentPage) : base("Home", "Backgrounds/HomeScreen/iw_app_ios_background_home.png", parentPage)
        {
            this.BackButton.Text = "< Logout";
            
            this.FormsLink = new HomePageLinkModel(PageType.Forms, "Forms", "Icons/HomeScreen/iw_app_ios_icon_form.png");
            this.PrepopLink = new HomePageLinkModel(PageType.Prepop, "Pre-Pop Forms", "Icons/HomeScreen/iw_app_ios_icon_prepop.png");
            this.HistoryLink = new HomePageLinkModel(PageType.HistoryAll, "History", "Icons/HomeScreen/iw_app_ios_icon_history.png");
            this.PendingLink = new HomePageLinkModel(PageType.HistoryPending, "Pending", "Icons/HomeScreen/iw_app_ios_icon_pending.png");
            this.ParkedLink = new HomePageLinkModel(PageType.HistoryParked, "Parked", "Icons/HomeScreen/iw_app_ios_icon_parked.png");
            this.AutosavedLink = new HomePageLinkModel(PageType.HistoryAutosaved, "Auto Save", "Icons/HomeScreen/iw_app_ios_icon_autosave.png");
            this.SentLink = new HomePageLinkModel(PageType.HistorySent, "Sent", "Icons/HomeScreen/iw_app_ios_icon_send.png");

            this.HomePageItems = new ObservableCollection<HomePageLinkModel>
            {
                FormsLink, PrepopLink, HistoryLink, PendingLink, ParkedLink, AutosavedLink, SentLink
            };

			this.HomeTable = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(HomePageLink)),
				ItemsSource = this.HomePageItems,
				SeparatorColor = Color.Black,
				SeparatorVisibility = SeparatorVisibility.Default,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HasUnevenRows = true
            };
            this.HomeTable.ItemTapped += (sender, eventArgs) =>
            {
                var item = (HomePageLinkModel)eventArgs.Item;

                switch (item.Page)
                {
                    case PageType.HistoryAll:
                    case PageType.HistoryAutosaved:
                    case PageType.HistoryParked:
                    case PageType.HistoryPending:
                    case PageType.HistorySent:
                        var histPage = new HistoryPage(item.Page, this);
                        App.Current.MainPage = histPage;
                        break;
                    case PageType.Forms:
                        var formList = new FormListPage(this);
                        App.Current.MainPage = formList;
                        break;
                    case PageType.Prepop:
                        App.Current.MainPage = new PrepopPage(this);
                        break;
                }

                this.HomeTable.SelectedItem = null;
            };

            this.PageContent.Content = this.HomeTable;
        }

        public void RefreshUser()
        {
            Task.Run(async () =>
            {
                var user = ((App)App.Current).LoggedInUser;

                // initial update of values
                int formsLinkCount = (await App.DatabaseHelper.GetAllFormsAsync(user)).Count;
                int prepopLinkCount = (await App.DatabaseHelper.GetPrepopForms(user)).Count;
                int historyLinkCount = (await App.DatabaseHelper.GetTransactionsAsync(user)).Count;
                int parkedLinkCount = (await App.DatabaseHelper.GetTransactionsAsync(user, DatabaseHelper.Status.Parked)).Count;
                int pendingLinkCount = (await App.DatabaseHelper.GetTransactionsAsync(user, DatabaseHelper.Status.Pending)).Count;
                int sentLinkCount = (await App.DatabaseHelper.GetTransactionsAsync(user, DatabaseHelper.Status.Sent)).Count;
                int autosavedLinkCount = (await App.DatabaseHelper.GetTransactionsAsync(user, DatabaseHelper.Status.Autosaved)).Count;

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.FormsLink.Indicator = formsLinkCount;
                    this.PrepopLink.Indicator = prepopLinkCount;
                    this.HistoryLink.Indicator = historyLinkCount;
                    this.ParkedLink.Indicator = parkedLinkCount;
                    this.PendingLink.Indicator = pendingLinkCount;
                    this.SentLink.Indicator = sentLinkCount;
                    this.AutosavedLink.Indicator = autosavedLinkCount;
                });

                //start updating
                this.refreshForms(user);
            });
        }

        private void refreshForms(User user)
        {
            var svc = App.WebService.GetServiceCenterClient();
            var getforms = new GetEforms(user.Username, user.Password);
            var text = getforms.ToXml().ToString();
            var encDate = Crypto.GetFormattedDate(DateTime.Now);
            svc.SendDataCompleted += async (sender, eventArgs) =>
            {
                if (eventArgs.Error != null)
                {
                    // error
                    return;
                }
                if (eventArgs.Cancelled)
                {
                    return;
                }
                var decrypted = Crypto.Decrypt(eventArgs.Data, eventArgs.Result);
                var response = new ResponseItem(XElement.Parse(decrypted));
                if (response.ErrorCode == 0)
                {
                    var formsXml = XElement.Parse(response.FileData);
                    var forms = new List<Form>();
                    var existingForms = await App.DatabaseHelper.GetAllFormsAllStatusAsync(user);
                    var newOrUpdated = new List<Form>();
                    foreach (var elem in formsXml.Elements("eform").ToList())
                    {
                        var appId = elem.Attribute("appkey").Value;
                        var name = elem.Element("name").Value;
                        var amended = elem.Element("amended").Value;
                        var amendedDate = DateFromAmended(amended);
                        var oldItem = existingForms.FirstOrDefault(x => x.FormIdentifier == appId);
                        if (oldItem != null)
                        {
                            existingForms.Remove(oldItem);
                            if (oldItem.UpdatedDate == amendedDate && oldItem.Status == (int)DatabaseHelper.Status.Available)
                            {
                                //newOrUpdated.Add(oldItem);
                                continue;
                            }
                            oldItem.UpdatedDate = amendedDate;
                            oldItem.FormName = name;
                            
                            oldItem.Status = (int)DatabaseHelper.FormStatus.Available;
                            if (oldItem.ParentFolder != -1)
                            {
                                var folder = await App.DatabaseHelper.GetFolderAsync(oldItem.ParentFolder);
                                if (folder == null)
                                {
                                    oldItem.ParentFolder = -1;
                                }
                            }
                            await App.DatabaseHelper.UpdateItemAsync(oldItem);
                            newOrUpdated.Add(oldItem);
                        }
                        else
                        {
                            //new item
                            var newForm = new Form
                            {
                                FormIdentifier = appId,
                                FormName = name,
                                ParentFolder = -1,
                                User = user.Id,
                                UpdatedDate = amendedDate,
                                Status = (int)DatabaseHelper.FormStatus.Available
                            };
                            newForm = await App.DatabaseHelper.AddForm(newForm);
                            newOrUpdated.Add(newForm);
                        }
                    }

                    this.updateForms(user, newOrUpdated);

                    foreach (var form in existingForms.Where(x => x.Status != (int)DatabaseHelper.FormStatus.Deleted).ToList())
                    {
                        // these were not found in the new list - delete them

                        await App.DatabaseHelper.DeletePrepopFormsForApp(form);

                        form.Status = (int)DatabaseHelper.FormStatus.Deleted;
                        await App.DatabaseHelper.UpdateItemAsync(form);
                        DependencyService.Get<IFormFileTools>().DeleteFormData(form.FormIdentifier, user.Username);
                    }

                    // intentionally created as vars - these will retrieve the database values in the
                    // thread created by the webservice repsonse, so the UI doesn't freeze...
                    // then we update the links in the UI thread after.
                    var allFormsNow = await App.DatabaseHelper.GetAllFormsAsync(user);
                    var prepopNow = await App.DatabaseHelper.GetPrepopForms(user);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        FormsLink.Indicator = allFormsNow.Count;
                        PrepopLink.Indicator = prepopNow.Count;
                    });

                    this.updatePrepops(user);
                }
                else if (response.ErrorCode == 101)
                {
                    ((App)App.Current).Logout();
                }
            };
            WebRequest request = null;
            App.TestWebAccess(svc.Endpoint.Address.Uri.ToString(), ref request, (result) =>
            {
                WebResponse response = null;
                try
                {
                    response = request.EndGetResponse(result);

                    svc.SendDataAsync(encDate, Crypto.Encrypt(getforms.ToXml().ToString(), encDate));
                }
                catch
                {
                    
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

		private bool prepopsLoadingDoneOnce = false;
		private bool prepopsDone = false;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.RefreshUser();
        }

        private async void updatePrepops(User user)
        {
			if (this.prepopsLoadingDoneOnce && !this.prepopsDone)
			{
				return;
			}
			this.prepopsLoadingDoneOnce = true;
			this.prepopsDone = false;
            var lastPrepopVersion = await App.DatabaseHelper.GetSettingAsync(user, "prepop_version", "-1");
            var prepopRequest = new GetEformPrepopDataForUser(user.Username, user.Password, int.Parse(lastPrepopVersion.Value));
            var svc = App.WebService.GetServiceCenterClient();
            svc.SendDataCompleted += async (sender, eventArgs) =>
            {
                if (eventArgs.Error != null)
                {
                    //error
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
					
                    var prepopData = XElement.Parse(response.FileData);
                    var wrapper = new PrepopWrapper(prepopData, response.PrepopVersion);
					await App.DatabaseHelper.SetSettingAsync(user, "prepop_version", wrapper.Version.ToString());
					this.prepopsDone = true;
                    var userForms = await App.DatabaseHelper.GetAllFormsAsync(user);
                    foreach (var add in wrapper.Additions)
                    {
                        var form = userForms.FirstOrDefault(f => f.FormIdentifier == add.AppKey.ToString());
                        if (form == null)
                        {
                            // no form, ignore this prepop
                            continue;
                        }
                        var existingForForm = await App.DatabaseHelper.GetPrepopForms(form);
                        foreach (var ppf in existingForForm.Where(ppf => ppf.Identifier == add.PrepopId))
                        {
                            //remove old copies of this prepop
                            await App.DatabaseHelper.DeletePrepopForm(ppf);
                        }

                        var newPrepop = new PrepopForm
                        {
                            Form = form.Id,
                            Name = add.PrepopName,
                            Identifier = add.PrepopId,
                            Status = 0,
                            VersionNumber = wrapper.Version,
                            User = user.Id
                        };
                        newPrepop = await App.DatabaseHelper.CreatePrepopForm(newPrepop);

                        foreach (var field in add.Fields)
                        {
                            var newField = new PrepopField
                            {
                                PrepopForm = newPrepop.Id,
                                FieldName = field.FieldName,
                                FieldValue = field.FieldValue
                            };
                            await App.DatabaseHelper.CreatePrepopField(newField);
                        }
                    }

                    var existing = await App.DatabaseHelper.GetPrepopForms(user);
                    foreach (var removal in wrapper.Removals)
                    {
                        var existingForm = existing.FirstOrDefault(f => f.Identifier == removal.PrepopId);
                        if (existingForm == null)
                        {
                            continue;
                        }
                        await App.DatabaseHelper.DeletePrepopForm(existingForm);
                    }

                    
                    var prepopsNow = await App.DatabaseHelper.GetPrepopForms(user);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        PrepopLink.Indicator = prepopsNow.Count;
                    });
                }
            };
            var encDate = Crypto.GetFormattedDate(DateTime.Now);

            WebRequest request = null;
            App.TestWebAccess(svc.Endpoint.Address.Uri.ToString(), ref request, (result) =>
            {
                WebResponse response = null;
                try
                {
                    response = request.EndGetResponse(result);
                    svc.SendDataAsync(encDate, Crypto.Encrypt(prepopRequest.ToXml().ToString(), encDate));
                }
                catch
                {

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

        private void updateForms(User user, List<Form> forms)
        {
            foreach (var form in forms)
            {
                var svc = App.WebService.GetServiceCenterClient();
                
                var getRequest = new GetZipFormSecure(user.Username, user.Password, int.Parse(form.FormIdentifier));
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
                        DependencyService.Get<IFormFileTools>().SaveAndUnzipFormFiles(form.FormIdentifier, user.Username, bytes);
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

        private static DateTime DateFromAmended(string amended)
        {
            return new DateTime(
                            int.Parse(amended.Substring(4, 4)),
                            int.Parse(amended.Substring(2, 2)),
                            int.Parse(amended.Substring(0, 2)),
                            int.Parse(amended.Substring(9, 2)),
                            int.Parse(amended.Substring(11, 2)),
                            int.Parse(amended.Substring(13, 2))
                            );
        }

        protected override async void GoBack()
        {
            var result = await this.DisplayAlert("Log out?", "Are you sure you would like to log out?", "Yes", "No");
            if (result)
            {
                ((App)App.Current).Home = null;
                ((App)App.Current).LoggedInUser = null;
                ((App)App.Current).Login = new LoginPage();
                App.Current.MainPage = ((App)App.Current).Login;
            }
        }

        public HomePageLinkModel FormsLink { get; set; } 
        public HomePageLinkModel PrepopLink { get; set; }
        public HomePageLinkModel HistoryLink { get; set; }
        public HomePageLinkModel PendingLink { get; set; }
        public HomePageLinkModel ParkedLink { get; set; }
        public HomePageLinkModel AutosavedLink { get; set; }
        public HomePageLinkModel SentLink { get; set; }

        public ObservableCollection<HomePageLinkModel> HomePageItems { get; set; }
        
        public ListView HomeTable { get; set; }
    }
}
