using Android.App;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;

namespace MentalArithmetic
{
    // <summary>Handler for the overview page view</summary>
    [Activity(Label = "Overview", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Overview : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Overview);

            MediaPlayer soundClap = MediaPlayer.Create(this, Resource.Raw.sound_clap);
            soundClap.Start();

            // Get their results.
            int score = Intent.GetIntExtra("Score", 0);
            int questions = Intent.GetIntExtra("Questions", 0);
            int wrong = Intent.GetIntExtra("Wrong", 0);
            int totalTime = Intent.GetIntExtra("TotalTime", 0);
            string difficulty = Intent.GetStringExtra("Difficulty");
            DifficultyLevel level = DifficultyLevelGet.FromString(difficulty);

            List<string> wrongQuestions = new List<string>();
            // If they got more than 0 wrong questions, get them.
            if (wrong > 0)
            {
                for (int i = 0; i < wrong; i++)
                {
                    wrongQuestions.Add(Intent.GetStringExtra(QuestionView.I_WRONG_Q + i));
                }
            }

            // improve w change header.

            // Update values with the gathered values.
            FindViewById<TextView>(Resource.Id.txtQOverview).Text = $"{score}/{questions}";
            FindViewById<TextView>(Resource.Id.txtTimeTaken).Text = $"In {totalTime}s";
            TextView modeData = FindViewById<TextView>(Resource.Id.txtMode);
            modeData.Text = $"In {difficulty} mode";
            modeData.SetTextColor(level.GetColor());

            TextView improvementsHeader = FindViewById<TextView>(Resource.Id.txtQImproveHeader);
            TextView improvements = FindViewById<TextView>(Resource.Id.txtQImprove0);
            // Clear any possible history.
            improvements.Text = "";

            if (wrongQuestions.Count == 0)
            {
                // If got 0 questions wrong, hide the header.
                improvementsHeader.Visibility = Android.Views.ViewStates.Invisible;
            }
            else
            {
                // Show the header incase hidden
                improvementsHeader.Visibility = Android.Views.ViewStates.Visible;
                // For each wrong question, append to the improvements text.
                wrongQuestions.ForEach(delegate (string wrongQuestion)
                {
                    improvements.Text += wrongQuestion + "\n";
                });
            }

            // When they want to start again, start DifficultySelect page view.
            FindViewById<Button>(Resource.Id.btnGoAgain).Click += delegate
            {
                soundClap.Stop();
                StartActivity(typeof(DifficultySelect));
            };

        }

    }
}