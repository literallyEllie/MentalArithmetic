using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using EllieLib;

namespace MentalArithmetic
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {

        private PageWrapper page = new PageWrapper();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            EasyView<Button> btnStart = new EasyView<Button>(page, FindViewById<Button>(Resource.Id.btnStart));
            btnStart.Click(delegate
            {
                StartActivity(typeof(DifficultySelect));
            });
        }


    }
}