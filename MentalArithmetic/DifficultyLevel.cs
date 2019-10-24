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

namespace MentalArithmetic
{
    enum DifficultyLevel
    {

        Easy,
        Medium,
        Hard

    }

    static class DifficultyLevelGet
    {

        public static String GetHex(this DifficultyLevel difficulty)
        {

            switch (difficulty)
            {
                case DifficultyLevel.Medium:
                    return "#ffe65100";
                case DifficultyLevel.Hard:
                    return "#ff880e4f";
                default:
                    return "#ff43a047";
            }

        }

        public static Color GetColor(this DifficultyLevel difficulty)
        {

            switch (difficulty)
            {
                case DifficultyLevel.Medium:
                    return new Color(255, 230, 81);
                case DifficultyLevel.Hard:
                    return new Color(255, 136, 14);
                default:
                    return new Color(255, 67, 160);
            }

        }

        public static DifficultyLevel FromString(String input)
        {
            foreach (DifficultyLevel level in Enum.GetValues(typeof(DifficultyLevel)))
            {
                if (level.ToString().Equals(input))
                {
                    return level;
                }
            }
            return DifficultyLevel.Easy;
        }

    }

}