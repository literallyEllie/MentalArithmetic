using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using EllieLib;

namespace MentalArithmetic
{
    // <summary>Class <c>DifficultySelect</c> handles the operations for the <c>DifficultySelect</c> page.</summary>
    [Activity(Label = "DifficultySelect", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DifficultySelect : Activity
    {

        private PageWrapper page;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DifficultySelect);

            // Since this page never changes during runtime, if the page is already setup, don't do it again.
            if (page != null) return;

            page = new PageWrapper();

            // Make new Intent to pass through to QuestionView
            Intent intent = new Intent(this, typeof(QuestionView));

            // For each button, add the intent key "Difficulty" and the respective DifficultyLevel seralized.
            new EasyView<Button>(page, FindViewById<Button>(Resource.Id.btnEasy), delegate
            {
                intent.PutExtra("Difficulty", DifficultyLevel.Easy.ToString());
                StartActivity(intent);
            });

            new EasyView<Button>(page, FindViewById<Button>(Resource.Id.btnMedium), delegate
            {
                intent.PutExtra("Difficulty", DifficultyLevel.Medium.ToString());
                StartActivity(intent);
            });

            new EasyView<Button>(page, FindViewById<Button>(Resource.Id.btnHard), delegate
            {
                intent.PutExtra("Difficulty", DifficultyLevel.Hard.ToString());
                StartActivity(intent);
            });

        }
    }

}