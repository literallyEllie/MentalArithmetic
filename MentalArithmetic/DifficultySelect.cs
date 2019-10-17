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
using EllieLib;

namespace MentalArithmetic
{
    [Activity(Label = "DifficultySelect")]
    public class DifficultySelect : Activity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DifficultySelect);

            // Create your application here
        }
    }

}