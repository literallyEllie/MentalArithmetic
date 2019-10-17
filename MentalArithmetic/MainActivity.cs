using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using EllieLib;

namespace MentalArithmetic
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        private AppWrapper appWrapper = new AppWrapper();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            EasyView<Button> btnStart = new EasyView<Button>(appWrapper, FindViewById<Button>(Resource.Id.btnStart));
            btnStart.Click(delegate
            {
                StartActivity(typeof(DifficultySelect));
            });
        }


    }
}