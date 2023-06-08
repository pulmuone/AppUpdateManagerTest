using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Xamarin.Google.Android.Play.Core.AppUpdate;
using Xamarin.Google.Android.Play.Core.Install;
using Xamarin.Google.Android.Play.Core.Install.Model;
using Xamarin.Google.Android.Play.Core.Tasks;

namespace AppUpdateManagerTest.Droid
{
    [Activity(Label = "AppUpdateManagerTest", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static IAppUpdateManager appUpdateManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public class AddOnSuccessListener : Java.Lang.Object, IOnSuccessListener
        {
            public void OnSuccess(Java.Lang.Object p0)
            {
                var p1 = p0.JavaCast<AppUpdateInfo>();

                System.Diagnostics.Debug.WriteLine(p1.AvailableVersionCode().ToString()); //Version number (정수)

                try
                {
                    if (p1.UpdateAvailability() == UpdateAvailability.UpdateAvailable && p1.IsUpdateTypeAllowed(AppUpdateType.Immediate))
                    {
                        // Request the update.
                        appUpdateManager.StartUpdateFlowForResult(p1, Xamarin.Essentials.Platform.CurrentActivity, AppUpdateOptions.NewBuilder(AppUpdateType.Immediate).Build(), 1001);
                    }
                }
                catch (System.Exception e)
                {

                }
            }
        }

        public class AddOnSuccessListener2 : Java.Lang.Object, IOnSuccessListener
        {
            public void OnSuccess(Java.Lang.Object p0)
            {
                var p1 = p0.JavaCast<AppUpdateInfo>();

                //AppInfo.BuildString; //정수형 build번호, Version Number
                System.Diagnostics.Debug.WriteLine(p1.AvailableVersionCode().ToString()); //Version number (정수)

                try
                {
                    //apk로 설치하면 아래 부분이 작동 되지 않음.
                    if (p1.UpdateAvailability() == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
                    {
                        appUpdateManager.StartUpdateFlowForResult(p1, Xamarin.Essentials.Platform.CurrentActivity, AppUpdateOptions.NewBuilder(AppUpdateType.Immediate).Build(), 1001);
                    }
                }
                catch (System.Exception e)
                {

                }
            }
        }



        public class AppUpdateInstallStateListener : Java.Lang.Object, IInstallStateUpdatedListener
        {
            public void OnStateUpdate(InstallState state)
            {
                //var p1 = state.JavaCast<AppUpdateInfo>();

                if (state.InstallStatus() == InstallStatus.Downloaded)
                {
                    appUpdateManager.CompleteUpdate();
                }
            }

            public void OnStateUpdate(Object p0)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}