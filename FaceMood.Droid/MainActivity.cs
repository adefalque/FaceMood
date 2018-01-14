using Android;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Plugin.Media;
using Plugin.Permissions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace FaceMood.Droid
{
    [Activity(Label = "FaceMood.Droid", MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);

            Acr.UserDialogs.UserDialogs.Init(this);

            // Set our view from the "main" layout resource
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

