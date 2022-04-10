using INKWRX_Mobile.Connect.Types;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;

namespace INKWRX_Mobile.Views
{
    public class LoginPage : ContentPage
    {
        public LoginPage()
        {

            #region Page Setup (General)

            var pageRel = new RelativeLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White
            };
            var bgImage = new Image
            {
                Aspect = Aspect.Fill,
                Source = CoreAppTools.GetImageSource("Backgrounds/LoginScreen/iw_app_ios_login_background.png")
            };
            pageRel.Children.Add(bgImage, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height; }));
            var scroll = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
            };
            pageRel.Children.Add(scroll, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height; }));
            var contentStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(20, Device.OnPlatform(40, 20, 20), 20, 20)
            };

            var middleStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            var bottomStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            contentStack.Children.Add(middleStack);
            contentStack.Children.Add(bottomStack);

            #endregion

            #region Page Setup (Controls)

            var aspect = 164d / 826f;
            var logo = new AutoHeightImageView(aspect)
            {
                Source = CoreAppTools.GetImageSource("Icons/LoginScreen/iw_app_ios_logo.png"),
                Aspect = Aspect.AspectFit,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            var usernameStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(10, 0),
                Margin = new Thickness(0, 50, 0, 0)
            };
            var passwordStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(10, 0)
            };
            var userline = new BoxView
            {
                Color = Color.White,
                HeightRequest = 1,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            var passline = new BoxView
            {
                Color = Color.White,
                HeightRequest = 1,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            var userImage = new Image
            {
                Source = CoreAppTools.GetImageSource("Icons/LoginScreen/iw_app_ios_icon_username.png"),
                WidthRequest = 20,
                HeightRequest = 20,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var passImage = new Image
            {
                Source = CoreAppTools.GetImageSource("Icons/LoginScreen/iw_app_ios_icon_password.png"),
                WidthRequest = 20,
                HeightRequest = 20,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            this.UserEntry = new BorderlessEntryView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(20,0,0,0),
                BackgroundColor = Color.Transparent,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                Keyboard = Keyboard.Create(KeyboardFlags.None)
            };
            this.PasswordEntry = new BorderlessEntryView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                IsPassword = true,
                Margin = new Thickness(20, 0, 0, 0),
                BackgroundColor = Color.Transparent,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.White
            };

            usernameStack.Children.Add(userImage);
            usernameStack.Children.Add(this.UserEntry);
            passwordStack.Children.Add(passImage);
            passwordStack.Children.Add(this.PasswordEntry);

            this.LoginButton = new Button
            {
                BackgroundColor = Color.FromRgb(77d / 255d, 134d / 255d, 142d / 255d),
                TextColor = Color.White,
                BorderWidth = 0,
                HeightRequest = 50,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Login"
            };
            this.LoginButton.Clicked += LoginButtonClicked;

            var rememberStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(5)
            };
            var rememberInnerStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                
            };

            this.RememberImage = new DoubleImageView(CoreAppTools.GetImageSource("Icons/LoginScreen/iw_app_ios_icon_boxtick.png"),
                CoreAppTools.GetImageSource("Icons/LoginScreen/iw_app_ios_icon_boxuntick.png"))
            {
                WidthRequest = 18,
                HeightRequest = 18,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFit,
            };

            var rememberLabel = new Label
            {
                TextColor = Color.White,
                Text = "Remember Password",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 13
            };
            
            rememberInnerStack.Children.Add(this.RememberImage);
            rememberInnerStack.Children.Add(rememberLabel);
            rememberStack.Children.Add(rememberInnerStack);
            var rememberTapped = new TapGestureRecognizer();
            rememberTapped.Tapped += (sender, eventArgs) =>
            {
                this.RememberImage.Toggle();
            };
            rememberStack.GestureRecognizers.Add(rememberTapped);
            var topPad = new StackLayout
            {
                VerticalOptions = LayoutOptions.StartAndExpand
            };
            var bottomPad = new StackLayout
            {
                VerticalOptions = LayoutOptions.EndAndExpand
            };

            var middleInnerStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            middleInnerStack.Children.Add(logo);
            middleInnerStack.Children.Add(usernameStack);
            middleInnerStack.Children.Add(userline);
            middleInnerStack.Children.Add(passwordStack);
            middleInnerStack.Children.Add(passline);
            middleInnerStack.Children.Add(rememberStack);
            middleInnerStack.Children.Add(this.LoginButton);

            middleStack.Children.Add(middleInnerStack);

            #endregion

            #region Page Setup (Infos)

            var bottomInnerStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = new Thickness(0, 0, 0, 50)
            };

            var devId = DependencyService.Get<IDeviceDetails>().GetDeviceId();
            //if (devId.Length > 25)
            //{
            //    devId = string.Format("{0}...",  devId.Substring(0, 25));
            //}
            var deviceLabel = new Label
            {
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = string.Format("Device ID: {0}", devId),
                FontSize = 10
            };

            var appVersion = DependencyService.Get<IDeviceDetails>().GetVersionNumber();
            var coreVersion = CoreAppTools.GetVersionNumber();
            var versionLabel = new Label
            {
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = string.Format("Version: {0} (Core: {1})", appVersion, coreVersion),
                FontSize = 10
            };
            var copyrightLabel = new Label
            {
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = string.Format("© 2017 Destiny Wireless", appVersion, coreVersion),
                FontSize = 10
            };
            var privacyLabel = new Label
            {
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = string.Format("Privacy Policy", appVersion, coreVersion),
                FontSize = 10
            };

            var privacyTap = new TapGestureRecognizer();
            privacyTap.Tapped += (sender, eventArgs) =>
            {
                Device.OpenUri(new Uri("https://www.inkwrx.com/website-privacy-policy/"));
            };

            privacyLabel.GestureRecognizers.Add(privacyTap);

            bottomInnerStack.Children.Add(deviceLabel);
            bottomInnerStack.Children.Add(versionLabel);
            bottomInnerStack.Children.Add(copyrightLabel);
            bottomInnerStack.Children.Add(privacyLabel);

            bottomStack.Children.Add(bottomInnerStack);

            #endregion

            var scrollRel = new RelativeLayout();
            scrollRel.Children.Add(contentStack, Constraint.RelativeToParent((parent) => { return parent.Width * 0.125; }), Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => { return parent.Width * 0.75; }), Constraint.RelativeToParent((parent) => { return parent.Height; }));
            scroll.Content = scrollRel;
            this.Content = pageRel;
            this.RetrieveLastLogin();
        }

        private async void RetrieveLastLogin()
        {
            var rememberSetting = await App.DatabaseHelper.GetSettingAsync("remember_login", "false");
            var remember = rememberSetting.Value == "true";
            this.RememberImage.IsOn = remember;
            if (remember)
            {
                var lastLogin = await App.DatabaseHelper.GetSettingAsync("last_login", "");
                var lastUser = await App.DatabaseHelper.GetUserAsync(lastLogin.Value);
                if (lastUser == null)
                {
                    return;
                }
                this.UserEntry.Text = lastUser.Username;
                this.PasswordEntry.Text = lastUser.Password;
            }
        }

        public BorderlessEntryView UserEntry { get; set; }
        public BorderlessEntryView PasswordEntry { get; set; }

        private void LoginButtonClicked(object sender, EventArgs eventArgs)
        {
            this.LoginButton.IsEnabled = false;
            var username = this.UserEntry.Text.Trim().ToLower();
            var password = this.PasswordEntry.Text.Trim();

            var svc = App.WebService.GetFormServiceClient();
            var encDate = Crypto.GetFormattedDate(DateTime.Now);
            var validate = new ValidateTablet(username, password);
            
            svc.SendDataCompleted += async (completedObject, completedEventArgs) =>
            {
                if (completedEventArgs.Error != null)
                {
                    var user = await App.DatabaseHelper.GetUserAsync(username);
                    if (user == null)
                    {
                        // can't login offline
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.DisplayAlert("Login Error", "Error logging in: " + completedEventArgs.Error.Message, "Ok");
                            this.LoginButton.IsEnabled = true;
                        });
                        return;
                    }
                    if (password == user.Password)
                    {
                        
                        await App.DatabaseHelper.SetSettingAsync("last_login", this.RememberImage.IsOn ? user.Username : "");
                        await App.DatabaseHelper.SetSettingAsync("remember_login", this.RememberImage.IsOn ? "true" : "false");
                        ((App)App.Current).LoggedInUser = user;
                        ((App)App.Current).FormSendingService = new Connect.FormSendingService(((App)App.Current).LoggedInUser);
                        ((App)App.Current).FormSendingService.StartSendingService();
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ((App)App.Current).Home = new HomePage(null);
                            App.Current.MainPage = ((App)App.Current).Home;
                        });
                    }
                    else
                    {
                        // bad user/password
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.DisplayAlert("Incorrect", "Username or password incorrect", "Ok");
                            this.LoginButton.IsEnabled = true;
                        });
                    }

                    return;
                }
                if (completedEventArgs.Cancelled)
                {
                    this.LoginButton.IsEnabled = true;
                    return;
                }
                var decrypted = Crypto.Decrypt(completedEventArgs.Data, completedEventArgs.Result);
                var decXml = XElement.Parse(decrypted);

                var errorcode = decXml.Element("errorcode").Value;
                if (errorcode == "0")
                {
                    //login ok
                    var user = await App.DatabaseHelper.GetUserAsync(username);
                    var exists = user != null;
                    if (user == null)
                    {
                        user = new Database.Entity.User
                        {
                            Username = username.ToLower(),
                            Password = password
                        };
                    }
                    await App.DatabaseHelper.SetSettingAsync("remember_login", this.RememberImage.IsOn ? "true" : "false");
                    user.Password = password;
                    user = exists ? await App.DatabaseHelper.UpdateItemAsync(user) : await App.DatabaseHelper.SaveUser(user.Username, user.Password);

                    await App.DatabaseHelper.SetSettingAsync("last_login", this.RememberImage.IsOn ? username : "");
                    
                    ((App)App.Current).LoggedInUser = user;
                    ((App)App.Current).FormSendingService = new Connect.FormSendingService(((App)App.Current).LoggedInUser);
                    ((App)App.Current).FormSendingService.StartSendingService();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ((App)App.Current).Home = new HomePage(null);
                        App.Current.MainPage = ((App)App.Current).Home;
                    });
                }
                else if (errorcode == "101")
                {
                    // bad user/password
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.DisplayAlert("Incorrect", "Username or password incorrect", "Ok");
                        this.LoginButton.IsEnabled = true;
                    });
                }
                else
                {
                    var message = decXml.Element("message").Value;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.DisplayAlert("Login Error " + errorcode, message, "Ok");
                        this.LoginButton.IsEnabled = true;
                    });

                }

            };
            WebRequest request = null;
            App.TestWebAccess(svc.Endpoint.Address.Uri.ToString(), ref request, async (result) =>
            {
                WebResponse response = null;
                try
                {
                    response = request.EndGetResponse(result);
                    svc.SendDataAsync(encDate, Crypto.Encrypt(validate.ToXml().ToString(), encDate));
                }
                catch
                {
                    var user = await App.DatabaseHelper.GetUserAsync(username);
                    if (user == null)
                    {
                        // can't login offline
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.DisplayAlert("Login Error", "Error logging in: No Access To Server", "Ok");
                            this.LoginButton.IsEnabled = true;
                        });
                        return;
                    }
                    if (password == user.Password)
                    {

                        await App.DatabaseHelper.SetSettingAsync("last_login", this.RememberImage.IsOn ? user.Username : "");
                        await App.DatabaseHelper.SetSettingAsync("remember_login", this.RememberImage.IsOn ? "true" : "false");
                        ((App)App.Current).LoggedInUser = user;
                        ((App)App.Current).FormSendingService = new Connect.FormSendingService(((App)App.Current).LoggedInUser);
                        ((App)App.Current).FormSendingService.StartSendingService();
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ((App)App.Current).Home = new HomePage(null);
                            App.Current.MainPage = ((App)App.Current).Home;
                        });
                    }
                    else
                    {
                        // bad user/password
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.DisplayAlert("Incorrect", "Username or password incorrect", "Ok");
                            this.LoginButton.IsEnabled = true;
                        });
                    }

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


        public Button LoginButton { get; set; }

        public DoubleImageView RememberImage { get; set; }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.LoginButton.IsEnabled = true;
        }
    }
}
