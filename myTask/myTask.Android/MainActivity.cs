using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using CarouselView.FormsPlugin.Droid;
using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Forms;
using ImageCircle.Forms.Plugin.Droid;

namespace myTask.Android
{
    [Activity(Label = "myTask", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            XF.Material.Droid.Material.Init(this, savedInstanceState);
            CarouselViewRenderer.Init();
            CachedImageRenderer.Init(true);
            ImageCircleRenderer.Init();
            var ignore = typeof(SvgCachedImage);
            LoadApplication(new App());
        }
    }
}