﻿using AgoraNavigator.Login;
#if __ANDROID__
using Android.Graphics;
using Plugin.CurrentActivity;
#endif
using System;
using System.IO;
using Xamarin.Forms;

namespace AgoraNavigator.Badge
{
    public class BadgePage : NavigationPage
    {
        public static BadgeMasterPage barCodeMasterPage;

        public BadgePage()
        {
            barCodeMasterPage = new BadgeMasterPage();
            Navigation.PushAsync(barCodeMasterPage);
        }
    }

    public class BadgeMasterPage : ContentPage
    {
        public BadgeMasterPage()
        {
            Title = "Badge";
            if (!Users.isUserLogged)
            {
                return;
            }
            Appearing += OnAppearing;
            Disappearing += OnDisappearing;
        }

        private View CreateView(string code)
        {
            var writer = new ZXing.Mobile.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.CODE_39,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 1200,
                    Height = 256,
                    Margin = 10
                }
            };

            var bitmap = writer.Write(code);

#if __ANDROID__
            var source = ImageSource.FromStream(() =>
            {
                MemoryStream ms = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            });
#endif

#if __IOS__
            var source = ImageSource.FromStream(() => bitmap.AsPNG().AsStream());
#endif

            Image m = new Image
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Aspect = Aspect.AspectFit,
                Source = source
            };

            return m;
        }

        private void OnDisappearing(object sender, EventArgs e)
        {
            SetBrightness(-1f);
        }

        public void OnAppearing(object sender, EventArgs e)
        {
            string barcodeValue = BuildBarcodeValue();
            View barcode = CreateView(barcodeValue);
            Label idLabel = new Label();
            idLabel.HorizontalTextAlignment = TextAlignment.Center;
            idLabel.Text = barcodeValue;
            idLabel.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));

            Frame frame = new Frame()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            frame.Content = barcode;

            StackLayout stack = new StackLayout();
            stack.Children.Add(idLabel);

            StackLayout layout = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
            };
            layout.Children.Add(frame);
            layout.Children.Add(stack);

            Content = layout;

            SetBrightness(1f);
        }

        private void SetBrightness(float brightness)
        {
#if __ANDROID__

            var window = CrossCurrentActivity.Current.Activity.Window;
            var attributesWindow = new Android.Views.WindowManagerLayoutParams();
            attributesWindow.CopyFrom(window.Attributes);
            attributesWindow.ScreenBrightness = brightness;
            window.Attributes = attributesWindow;

#endif
        }

        private string BuildBarcodeValue()
        {
            return "220-" + Users.loggedUser.Id.ToString().PadLeft(4, '0');
        }
    }
}
