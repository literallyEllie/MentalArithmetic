
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MentalArithmetic
{
    [Activity(Label = "QuestionView")]
    public class QuestionView : Activity
    {

        static Random random = new Random();

        public enum Operator { Add, Subtract, Multiply, Divide };

        private Dictionary<Operator, String> operators = new Dictionary<Operator, String>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.QuestionView);

            operators.Add(Operator.Add, "+");
            operators.Add(Operator.Subtract, "-");
            operators.Add(Operator.Multiply, "*");
            operators.Add(Operator.Divide, "÷");

            int randIndex = random.Next(operators.Count);

            RandomQuestion randomQuestion = new RandomQuestion
            {
                OperatorSet = operators.ElementAt(randIndex)
            };
        }

        public class RandomQuestion
        {
            public KeyValuePair<Operator, String> OperatorSet { get; set; }

            private int numOne, numTwo;

            public RandomQuestion()
            {
                numOne = random.Next(0, 12);
                numTwo = random.Next(0, 12);
            }

            public String GetEquation() => $"{numOne} {OperatorSet.Value} {numTwo} = ?";

            public int GetAnswer()
            {
                switch (OperatorSet.Key)
                {
                    case Operator.Add:
                        return numOne + numTwo;
                    case Operator.Subtract:
                        return numOne - numTwo;
                    case Operator.Multiply:
                        return numOne * numTwo;
                    case Operator.Divide:
                        return numOne / numTwo;
                }

                return 0;
            }

        }
    }


}