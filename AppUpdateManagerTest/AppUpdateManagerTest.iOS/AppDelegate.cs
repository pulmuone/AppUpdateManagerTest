using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Foundation;
using Newtonsoft.Json.Linq;
using Plugin.StoreReview;
using UIKit;
using Xamarin.Essentials;

namespace AppUpdateManagerTest.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            VersionTracking.Track();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        //https://developer.apple.com/documentation/uikit/app_and_environment/managing_your_app_s_life_cycle
        public override void OnActivated(UIApplication application)
        {
            Console.WriteLine("OnActivated called, App is active.");
        }

        public override async void WillEnterForeground(UIApplication application)
        {
            Console.WriteLine("App will enter foreground");
            Version versionClient = new Version(VersionTracking.CurrentVersion);
            Version versionServer;

            string result;
            string version;

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri($"http://itunes.apple.com/lookup?bundleId={AppInfo.PackageName}"));
                    response.EnsureSuccessStatusCode();

                    result = await response.Content.ReadAsStringAsync();

                    JObject jo = new JObject();
                    try
                    {
                        jo = JObject.Parse(result);

                        if (jo.ContainsKey("resultCount"))
                        {
                            if (jo["resultCount"].ToString().Equals("1"))
                            {
                                JArray jsonArray = JArray.Parse(jo["results"].ToString());

                                foreach (JObject obj in jsonArray.Children<JObject>())
                                {
                                    if (obj.ContainsKey("version"))
                                    {
                                        //Debug.WriteLine(obj["version"].ToString());
                                        version = obj["version"].ToString();
                                        versionServer = new Version(version);

                                        if (versionServer > versionClient)
                                        {
                                            var trackId = obj["trackId"].ToString();
                                            CrossStoreReview.Current.OpenStoreListing(trackId);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public override void OnResignActivation(UIApplication application)
        {
            Console.WriteLine("OnResignActivation called, App moving to inactive state.");
        }

        public override void DidEnterBackground(UIApplication application)
        {
            Console.WriteLine("App entering background state.");
        }

        // not guaranteed that this will run
        public override void WillTerminate(UIApplication application)
        {
            Console.WriteLine("App is terminating.");
        }
    }
}
