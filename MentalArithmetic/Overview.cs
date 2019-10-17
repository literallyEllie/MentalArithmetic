using Android.App;
using Android.OS;

namespace MentalArithmetic
{
    [Activity(Label = "Overview")]
    public class Overview : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Overview);

            // Create your application here
        }
    }
}