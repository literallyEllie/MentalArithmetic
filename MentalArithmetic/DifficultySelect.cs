using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using EllieLib;

namespace MentalArithmetic
{
    [Activity(Label = "DifficultySelect", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DifficultySelect : Activity
    {

        private PageWrapper page;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DifficultySelect);

            if (page != null) return;

            page = new PageWrapper();

            Intent intent = new Intent(this, typeof(QuestionView));

            new EasyView<Button>(page, FindViewById<Button>(Resource.Id.btnEasy))
                .Click(delegate
            {
                intent.PutExtra("Difficulty", DifficultyLevel.Easy.ToString());
                StartActivity(intent);
            });

            new EasyView<Button>(page, FindViewById<Button>(Resource.Id.btnMedium))
                .Click(delegate
            {
                intent.PutExtra("Difficulty", DifficultyLevel.Medium.ToString());
                StartActivity(intent);
            });

            new EasyView<Button>(page, FindViewById<Button>(Resource.Id.btnHard))
                .Click(delegate
            {
                intent.PutExtra("Difficulty", DifficultyLevel.Hard.ToString());
                StartActivity(intent);
            });

        }
    }

}