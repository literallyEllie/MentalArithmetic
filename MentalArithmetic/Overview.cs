using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;

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

            List<string> wrongQuestions = new List<string>();
            if (wrong > 0)
            {
                for (int i = 0; i < wrong; i++)
                {
                    wrongQuestions.Add(Intent.GetStringExtra(QuestionView.I_WRONG_Q + i));
                }
            }

            FindViewById<TextView>(Resource.Id.txtQOverview).Text = $"{score}/{questions}";
            FindViewById<TextView>(Resource.Id.txtTimeTaken).Text = $"In {totalTime}s";
            TextView modeData = FindViewById<TextView>(Resource.Id.txtMode);
            modeData.Text = $"In {difficulty} mode";
            modeData.SetTextColor(level.GetColor());

            TextView improvementsHeader = FindViewById<TextView>(Resource.Id.txtQImproveHeader);
            TextView improvements = FindViewById<TextView>(Resource.Id.txtQImprove0);
            improvements.Text = "";
            if (wrongQuestions.Count == 0)
            {
                improvementsHeader.Visibility = Android.Views.ViewStates.Invisible;
            }
            else
            {
                improvementsHeader.Visibility = Android.Views.ViewStates.Visible;
                wrongQuestions.ForEach(delegate (string wrongQuestion)
                {
                    improvements.Text += wrongQuestion + "\n";
                });
            }

            FindViewById<Button>(Resource.Id.btnGoAgain).Click += delegate
            {
                StartActivity(typeof(DifficultySelect));
            };

        }

    }
}