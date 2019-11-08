using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using EllieLib;

namespace MentalArithmetic
{
    // <summary>Starting page where when they first open the app</summary>
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {

        private PageWrapper page;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // If page exists, return.
            if (page != null)
                return;

            // Make new page.
            page = new PageWrapper();

            // Button to begin.
            new EasyView<Button>(page, FindViewById<Button>(Resource.Id.btnStart), delegate
            {
                StartActivity(typeof(DifficultySelect));
            });
        }

    }
}