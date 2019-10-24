using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;

namespace MentalArithmetic
{
    [Activity(Label = "Overview")]
    public class Overview : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Overview);

            int score = Intent.GetIntExtra("Score", 0);
            int questions = Intent.GetIntExtra("Questions", 0);
            int wrong = Intent.GetIntExtra("Wrong", 0);
            int totalTime = Intent.GetIntExtra("TotalTime", 0);
            string difficulty = Intent.GetStringExtra("Difficulty");
            DifficultyLevel level = DifficultyLevelGet.FromString(difficulty);

            FindViewById<TextView>(Resource.Id.txtQOverview).Text = $"{score}/{questions}";
            FindViewById<TextView>(Resource.Id.txtTimeTaken).Text = $"In {totalTime}s";
            TextView modeData = FindViewById<TextView>(Resource.Id.txtMode);
            modeData.Text = $"In {difficulty} mode";
            modeData.SetTextColor(level.GetColor());

            FindViewById<Button>(Resource.Id.btnGoAgain).Click += delegate
            {
                StartActivity(typeof(DifficultySelect));
            };

        }

    }
}