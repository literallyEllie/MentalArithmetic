using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Widget;
using EllieLib;

namespace MentalArithmetic
{
    // <summary>The handler for the question view page. 
    // Where most things happen.</summary>
    [Activity(Label = "QuestionView", ScreenOrientation = ScreenOrientation.Portrait)]
    public class QuestionView : Activity
    {

        // Constants.
        public static readonly string I_WRONG_Q = "RQ_";
        private static readonly int MAX_QUESTIONS = 10;
        private static Random random = new Random();

        // Declare the possible operators a question could contain.
        public enum Operator { Add, Subtract, Multiply, Divide };

        // Operators with their display.
        private Dictionary<Operator, string> operators = new Dictionary<Operator, string>();

        // Page handle.
        private PageWrapper page;

        // Default difficulty level;
        private DifficultyLevel difficultyLevel = DifficultyLevel.Easy;

        // Score tracking.
        private int score;
        private int currentQuestionNumber = 0;

        // Question tracking.
        private RandomQuestion currentQuestion;
        private List<RandomQuestion> wrongQuestions;

        // Display elements.
        private EasyView<TextView> txtQuestionCounter, txtScore, txtTimeRemaining, txtQuestionBig;
        private EasyView<EditText> inAnswer;
        private EasyView<Button> btnSubmit;
        private EasyView<ImageView> imgRightWrong;

        // Sounds
        private MediaPlayer soundRight, soundWrong;

        // Colors for countdowns.
        private Color remainingColorGood = new Color(40, 54, 147),
            remainingColorBad = new Color(106, 27, 154),
            remainingColorNone = new Color(213, 0, 0);

        // Time tracking.
        private EasyTimer easyTimer;
        private int totalElapsedTime;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.QuestionView);

            // Parse difficulty from Intent.
            string difficulty = Intent.GetStringExtra("Difficulty");
            this.difficultyLevel = DifficultyLevelGet.FromString(difficulty);

            // If timer exists, stop and dispose so a new one can be created.
            if (this.easyTimer != null)
            {
                this.easyTimer.Stop();
                this.easyTimer.Dispose();
                this.easyTimer = null;
            }

            // Assign a countdown/up depending on the difficulty level.
            switch (this.difficultyLevel)
            {
                case DifficultyLevel.Easy:
                    // Want to keep track of time they are spending even if not countdown.
                    this.easyTimer = new EasyTimer(EasyTimer.TimerType.Up, 1000);
                    break;
                case DifficultyLevel.Medium:
                    // They have 20 seconds (20000ms) to answer each question.
                    this.easyTimer = new EasyTimer(EasyTimer.TimerType.Down, 1000, 20, 0);
                    break;
                case DifficultyLevel.Hard:
                    // They have 10 seconds (10000ms) to answer each question.
                    this.easyTimer = new EasyTimer(EasyTimer.TimerType.Down, 1000, 10, 0);
                    break;
            }

            // Load operators.
            if (operators.Count == 0)
            {
                operators.Add(Operator.Add, "+");
                operators.Add(Operator.Subtract, "-");
                operators.Add(Operator.Multiply, "*");
                operators.Add(Operator.Divide, "÷");
            }

            // Load page.
            if (page == null)
            {
                LoadPage();
            }
            else
            {
                // Since the page exists, reset all the values.
                this.score = 0;
                this.currentQuestion = null;
                this.totalElapsedTime = 0;
                this.UpdateTextFields();
                this.wrongQuestions.Clear();
            }

            // If they easy, don't show them the time remaining.
            if (this.difficultyLevel == DifficultyLevel.Easy)
            {
                this.txtTimeRemaining.Hide(true);
            }
            else
            {
                this.txtTimeRemaining.Show();

                // Assign countdown logic.
                this.easyTimer.TargetReachedEvent += delegate
                {
                    // If they've answered or the time is already up, ignore when the target reaches, even if it shoudln't.
                    if (this.currentQuestion.Answered && !this.currentQuestion.TimeUp) return;

                    // Register the time is up and add register as wrong question.
                    this.currentQuestion.TimeUp = true;
                    this.wrongQuestions.Add(this.currentQuestion);
                };

            }

            // Everytime the timer ticks regardless of difficulty.
            this.easyTimer.Tick(delegate
            {
                // If there is no current question or it is answered, ignore.
                if (this.currentQuestion != null && this.currentQuestion.Answered) return;

                    // If their time is up.
                    if (this.currentQuestion != null && this.currentQuestion.TimeUp)
                {
                    this.inAnswer.AllowInteraction(false);
                    this.txtTimeRemaining.Color(this.remainingColorNone);
                    this.txtTimeRemaining.Text("TIMEUP!");
                    this.btnSubmit.Text("Next Question");
                }
                else
                {
                    // Track elapsed time as they are still answering the question.
                    // If easy, txtTimeRemaining is hidden so doesn't matter to set text.
                    this.txtTimeRemaining.Text($"{this.easyTimer.Value}s remaining");
                    this.totalElapsedTime++;

                    this.UpdateRemainingColor();
                }
            });

            NextQuestion();
        }

        // <summary>Sets the next question, or if the <c>MAX_QUESTIONS</c> is reached it will prepare for going to the Overview page view.</summary>
        public void NextQuestion()
        {
            // If the next question is the max amount of questions to ask, go to overview.
            if (this.currentQuestionNumber++ == MAX_QUESTIONS)
            {
                this.GoToOverview();
                return;
            }

            // Hide the image saying if they are right or wrong.
            this.imgRightWrong.Hide(true);
            // Get new question.
            this.currentQuestion = GetNewQuestion();

            // Restart timer.
            if (this.easyTimer != null)
            {
                this.easyTimer.Restart();
            }

            // Update text fields for the new question.
            this.UpdateTextFields();
        }

        // <summary>Gets a new <c>RandomQuestion</c>.</summary>
        public RandomQuestion GetNewQuestion()
        {
            // Gets random index from all the operators.
            int randIndex = random.Next(operators.Count);

            // Makes new question with the random operator.
            RandomQuestion randomQuestion = new RandomQuestion(
                this.operators.ElementAt(randIndex)
            );

            return randomQuestion;
        }

        // <summary>Loads the page display elements</summary>
        private void LoadPage()
        {
            this.page = new PageWrapper();
            this.wrongQuestions = new List<RandomQuestion>();

            this.txtQuestionCounter = new EasyView<TextView>(page, FindViewById<TextView>(Resource.Id.txtQuestionCounter));
            this.txtScore = new EasyView<TextView>(page, FindViewById<TextView>(Resource.Id.txtScore));
            this.txtTimeRemaining = new EasyView<TextView>(page, FindViewById<TextView>(Resource.Id.txtTimeRemaining));
            this.txtQuestionBig = new EasyView<TextView>(page, FindViewById<TextView>(Resource.Id.txtQuestionBig));
            this.inAnswer = new EasyView<EditText>(page, FindViewById<EditText>(Resource.Id.inputAnswer));

            this.imgRightWrong = new EasyView<ImageView>(page, FindViewById<ImageView>(Resource.Id.imgRightWrong));

            this.soundRight = page.AddSound(Resource.Raw.sound_right, this);
            this.soundWrong = page.AddSound(Resource.Raw.sound_wrong, this);

            // Variable name submit does not always reflect the text. 
            this.btnSubmit = new EasyView<Button>(page, FindViewById<Button>(Resource.Id.btnSubmit), delegate
                {

                    // If their time is up, or they have answered, get the next question
                    if (this.currentQuestion.TimeUp || this.currentQuestion.Answered)
                    {
                        this.NextQuestion();
                        return;
                    }

                    // Ignore if input is empty.
                    if (this.inAnswer.IsEmpty()) return;

                    // Get the numerical answer.
                    int answer = Parse(this.inAnswer.Text());
                    // Stop interaction so they can't change their answer after submitting.
                    this.inAnswer.AllowInteraction(false);

                    // Register it is answered and update the "submit" button respectively.
                    this.currentQuestion.Answered = true;
                    this.btnSubmit.Text(this.currentQuestionNumber == MAX_QUESTIONS ? "See how you did!" : "Next Question");

                    // If they got it right.
                    if (this.currentQuestion.GetAnswer() == answer)
                    {
                        // Increment and update score.
                        this.score++;
                        this.txtScore.Text($"Score: {this.score}");
                        // Display image for when they get it right.
                        this.imgRightWrong.ImageSource(Resource.Drawable.RIGHT);
                        this.soundRight.Start();
                    }
                    else
                    {
                        // They got it wrong, so register as wrong question.
                        this.wrongQuestions.Add(this.currentQuestion);
                        // Display image for when they get it wrong.
                        this.imgRightWrong.ImageSource(Resource.Drawable.WRONG);
                        this.soundWrong.Start();
                    }

                    // Show if they got right or wrong.
                    this.imgRightWrong.Show();
                });
        }

        // <summary>Method to prepare user performance and pass it on the Overview page.</summary>
        private void GoToOverview()
        {
            // Create new intent with their data.
            Intent intent = new Intent(this, typeof(Overview));
            intent.PutExtra("Score", this.score);
            intent.PutExtra("Questions", MAX_QUESTIONS);
            intent.PutExtra("TotalTime", this.totalElapsedTime);
            intent.PutExtra("Difficulty", this.difficultyLevel.ToString());

            // Declare how many wrong questions they will have so Overview will know how many to expect.
            intent.PutExtra("Wrong", this.wrongQuestions.Count);

            // If they got more than 0 questions wrong.
            if (this.wrongQuestions.Count > 0)
            {
                for (int i = 0; i < this.wrongQuestions.Count; i++)
                {
                    RandomQuestion wrongQuestion = this.wrongQuestions[i];
                    String equation = wrongQuestion.GetEquation();

                    // Construct a correct equation by removing the "?" at the end of the equation and appending the correct answer.
                    intent.PutExtra(I_WRONG_Q + i, equation.Remove(equation.Length - 1) + wrongQuestion.GetAnswer());
                }
            }
            this.soundRight.Stop();
            this.soundWrong.Stop();            

            // Change page.
            StartActivity(intent);
        }

        // <summary>Parses a string <c>input</c> to integer. 
        // If it is not an integer, return the lowest integer value.</summary>
        private int Parse(String input)
        {
            if (!Int32.TryParse(input, out int parsed))
            {
                return int.MinValue;
            }

            return parsed;
        }

        // <summary>Update text fields for all variables.</summary>
        private void UpdateTextFields()
        {
            this.txtQuestionCounter.Text($"{this.currentQuestionNumber}/{MAX_QUESTIONS}");
            this.txtQuestionBig.Text(this.currentQuestion.GetEquation());
            this.txtScore.Text($"Score: {this.score}");
            this.inAnswer.Text("");
            this.inAnswer.AllowInteraction(true);
            this.btnSubmit.Text("Submit");
            // Show their remaining time if is down.
            if (this.easyTimer != null && this.easyTimer.GetTimerType() == EasyTimer.TimerType.Down)
            {
                this.txtTimeRemaining.Text($"{this.easyTimer.Value}s remaining");
                this.UpdateRemainingColor();
            }
        }

        // <summary>Update the remaining color based on what it is.</summary
        private void UpdateRemainingColor()
        {
            // If the current value is less than half the initial value, show bad time.
            if (this.easyTimer.Value < this.easyTimer.GetInitialValue() / 2)
            {
                this.txtTimeRemaining.Color(this.remainingColorBad);
            }
            else
            {
                this.txtTimeRemaining.Color(this.remainingColorGood);
            }
        }

        // <summary>The class for the generation of random questions</summary>
        public class RandomQuestion
        {
            // The selected operator for this question.
            public KeyValuePair<Operator, String> OperatorSet { get; set; }

            // The calculation values.
            private int left, right;
            // Answer is a double so it can be used for checking if the answer is acceptable.
            private double answer;

            // State trackers.
            public bool Answered { get; set; }
            public bool TimeUp { get; set; }

            public RandomQuestion(KeyValuePair<Operator, String> operatorSet)
            {
                this.OperatorSet = operatorSet;

                // Generate a new set of numbers until it is not a decimal and greater than -1.
                do
                {
                    NewValueSet();
                } while (!(Math.Floor(this.answer) == this.answer && this.answer >= 0));
                
            }

            // <summary>Generates a new value set with values between 1 and 12 and set the answer.</summary>
            public void NewValueSet()
            {
                this.left = random.Next(1, 12);
                this.right = random.Next(1, 12);
                this.answer = this.CalculateAnswer();
            }
 
            // <summary>Construct the unknown equation.</summary>
            public String GetEquation() => $"{left} {OperatorSet.Value} {right} = ?";

            // <summary>Returns the answer as an integer.</summary>
            public int GetAnswer()
            {
                return (int) answer;
            }

            // <summary>Calculates the answer based on the given operator.</summary>
            public double CalculateAnswer()
            {
                switch (OperatorSet.Key)
                {
                    case Operator.Add:
                        return left + right;
                    case Operator.Subtract:
                        return left - right;
                    case Operator.Multiply:
                        return left * right;
                    case Operator.Divide:
                        return (double) left / right;
                }

                // Return 0 if unknown.
                return 0;
            }
        }

    }
}