
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EllieLib;

namespace MentalArithmetic
{
    [Activity(Label = "QuestionView")]
    public class QuestionView : Activity
    {

        public static readonly String I_WRONG_Q = "RQ_";

        private static Random random = new Random();
        private static readonly int MAX_QUESTIONS = 10;

        public enum Operator { Add, Subtract, Multiply, Divide };

        private Dictionary<Operator, String> operators = new Dictionary<Operator, String>();

        private PageWrapper page;

        private DifficultyLevel difficultyLevel = DifficultyLevel.Easy;

        private int score;
        private int currentQuestionNumber = 0;
        private RandomQuestion currentQuestion;

        private List<RandomQuestion> wrongQuestions;

        private EasyView<TextView> txtQuestionCounter, txtScore, txtTimeRemaining, txtQuestionBig;
        private EasyView<EditText> inAnswer;
        private EasyView<Button> btnSubmit;
        private EasyView<ImageView> imgRightWrong;

        private Color remainingColorGood = new Color(40, 54, 147),
            remainingColorBad = new Color(106, 27, 154),
            remainingColorNone = new Color(213, 0, 0);

        private EasyTimer easyTimer;
        private int totalElapsedTime;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.QuestionView);

            string difficulty = Intent.GetStringExtra("Difficulty");
            this.difficultyLevel = DifficultyLevelGet.FromString(difficulty);

            // Timer
            if (this.easyTimer != null)
            {
                this.easyTimer.Stop();
                this.easyTimer.Dispose();
                this.easyTimer = null;
            }

            switch (this.difficultyLevel)
            {
                case DifficultyLevel.Easy:
                    this.easyTimer = new EasyTimer(EasyTimer.TimerType.Up, 1000);
                    break;
                case DifficultyLevel.Medium:
                    this.easyTimer = new EasyTimer(EasyTimer.TimerType.Down, 1000, 20, 0);
                    break;
                case DifficultyLevel.Hard:
                    this.easyTimer = new EasyTimer(EasyTimer.TimerType.Down, 1000, 10, 0);
                    break;
            }

            // Load operators
            if (operators.Count == 0)
            {
                operators.Add(Operator.Add, "+");
                operators.Add(Operator.Subtract, "-");
                operators.Add(Operator.Multiply, "*");
                operators.Add(Operator.Divide, "÷");
            }

            // Load page
            if (page == null)
            {
                LoadPage();
            }
            else
            {
                this.score = 0;
                this.currentQuestion = null;
                this.totalElapsedTime = 0;
                this.UpdateTextFields();
                this.wrongQuestions.Clear();
            }

            if (this.difficultyLevel == DifficultyLevel.Easy)
            {
                this.txtTimeRemaining.Hide(true);
            }
            else
            {
                this.txtTimeRemaining.Show();

                this.easyTimer.TargetReachedEvent += delegate
                {
                    if (this.currentQuestion.Answered) return;

                    this.currentQuestion.TimeUp = true;
                };

            }

            this.easyTimer.Tick(delegate
            {
                if (this.currentQuestion != null && this.currentQuestion.Answered) return;

                    // If no time
                    if (this.currentQuestion != null && this.currentQuestion.TimeUp)
                {
                    this.inAnswer.AllowInteraction(false);
                    this.txtTimeRemaining.Color(this.remainingColorNone);
                    this.txtTimeRemaining.Text("TIMEUP!");
                    this.btnSubmit.Text("Next Question");
                }
                else
                {
                    this.txtTimeRemaining.Text($"{this.easyTimer.Value}s remaining");
                    this.totalElapsedTime++;

                    this.UpdateRemainingColor();
                }
            });



            NextQuestion();
        }

        public void NextQuestion()
        {
            if (this.currentQuestionNumber++ == MAX_QUESTIONS)
            {
                this.GoToOverview();
                return;
            }

            this.imgRightWrong.Hide(true);
            this.currentQuestion = GetNewQuestion();

            // timer
            if (this.easyTimer != null)
            {
                this.easyTimer.Restart();
            }

            // update text fields
            this.UpdateTextFields();
        }

        public RandomQuestion GetNewQuestion()
        {
            int randIndex = random.Next(operators.Count);

            RandomQuestion randomQuestion = new RandomQuestion(
                this.operators.ElementAt(randIndex)
            );

            return randomQuestion;
        }

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

            this.btnSubmit = new EasyView<Button>(page, FindViewById<Button>(Resource.Id.btnSubmit));
            this.btnSubmit.Click(delegate
                {

                    if (this.currentQuestion.TimeUp || this.currentQuestion.Answered)
                    {
                        this.NextQuestion();
                        return;
                    }

                    if (this.inAnswer.Text().Trim() == ""
                        || this.currentQuestion.TimeUp) return;

                    int answer = Parse(this.inAnswer.Text());
                    this.inAnswer.AllowInteraction(false);

                    this.currentQuestion.Answered = true;
                    this.btnSubmit.Text(this.currentQuestionNumber  == MAX_QUESTIONS ? "See how you did!" : "Next Question");
                    this.imgRightWrong.Show();

                    if (this.currentQuestion.GetAnswer() == answer)
                    {
                        // right
                        this.score++;
                        this.txtScore.Text($"Score: {this.score}");
                        this.imgRightWrong.ImageSource(Resource.Drawable.RIGHT);
                    } else
                    {
                        // wrong
                        this.wrongQuestions.Add(this.currentQuestion);
                        this.imgRightWrong.ImageSource(Resource.Drawable.WRONG);
                    }
                });

        }

        private void GoToOverview()
        {
            Intent intent = new Intent(this, typeof(Overview));
            intent.PutExtra("Score", this.score);
            intent.PutExtra("Questions", MAX_QUESTIONS);
            intent.PutExtra("TotalTime", this.totalElapsedTime);
            intent.PutExtra("Difficulty", this.difficultyLevel.ToString());

            intent.PutExtra("Wrong", this.wrongQuestions.Count);

            if (this.wrongQuestions.Count > 0)
            {

                for (int i = 0; i < this.wrongQuestions.Count; i++)
                {
                    RandomQuestion wrongQuestion = this.wrongQuestions[i];

                    String equation = wrongQuestion.GetEquation();

                    intent.PutExtra(I_WRONG_Q + i, equation.Remove(equation.Length - 1) + wrongQuestion.GetAnswer());
                }

            }

            StartActivity(intent);
        }

        private int Parse(String input)
        {
            if (!Int32.TryParse(input, out int parsed))
            {
                return int.MinValue;
            }

            return parsed;
        }

        private void UpdateTextFields()
        {
            this.txtQuestionCounter.Text($"{this.currentQuestionNumber}/{MAX_QUESTIONS}");
            this.txtQuestionBig.Text(this.currentQuestion.GetEquation());
            this.txtScore.Text($"Score: {this.score}");
            this.inAnswer.Text("");
            this.inAnswer.AllowInteraction(true);
            this.btnSubmit.Text("Submit");
            if (this.easyTimer != null)
            {
                this.txtTimeRemaining.Text($"{this.easyTimer.Value}s remaining");
                this.UpdateRemainingColor();
            }
        }

        private void UpdateRemainingColor()
        {
            if (this.easyTimer.Value < this.easyTimer.GetInitialValue() / 2)
            {
                this.txtTimeRemaining.Color(this.remainingColorBad);
            }
            else
            {
                this.txtTimeRemaining.Color(this.remainingColorGood);
            }
        }

        public class RandomQuestion
        {
            public KeyValuePair<Operator, String> OperatorSet { get; set; }

            private int left, right;
            private double answer;

            public bool Answered { get; set; }
            public bool TimeUp { get; set; }

            public RandomQuestion(KeyValuePair<Operator, String> operatorSet)
            {
                this.OperatorSet = operatorSet;

                do
                {
                    NewValueSet();
                } while (!(Math.Floor(this.answer) == this.answer && this.answer >= 0));
                
            }

            public void NewValueSet()
            {
                this.left = random.Next(1, 12);
                this.right = random.Next(1, 12);
                this.answer = this.CalculateAnswer();
            }
 
            public String GetEquation() => $"{left} {OperatorSet.Value} {right} = ?";

            public int GetAnswer()
            {
                return (int) answer;
            }

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

                return 0;
            }
        }
    }


}