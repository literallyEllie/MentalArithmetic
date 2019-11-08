using System;

using Android.Graphics;

namespace MentalArithmetic
{
    // <summary>Class <c>DifficultyLevel</c> is an enum listing all the possible difficulties 
    // the user could take.</summary>
    enum DifficultyLevel
    {

        Easy,
        Medium,
        Hard

    }

    // <summary>Class <c>DifficultyLevelGet</c> is a companion to <c>DifficultyLevel</c>
    // and contains methods about it.</summary>
    static class DifficultyLevelGet
    {

        // <summary>Method <c>GetColor</c> gets the assosiated Color with the selected <c>DifficultyLevel</c></summary>
        public static Color GetColor(this DifficultyLevel difficulty)
        {
            switch (difficulty)
            {
                case DifficultyLevel.Medium:
                    return new Color(230, 81, 0);
                case DifficultyLevel.Hard:
                    return new Color(136, 14, 79);
                // Easy
                default:
                    return new Color(67, 160, 71);
            }
        }

        // <summary>Method <c>FromString</c> allows parsing from a string <c>input</c> to a <c>DifficultyLevel</c>
        // If the input is invalid, it is returned as <c>DifficultyLevel#Easy</c></summary>
        public static DifficultyLevel FromString(String input)
        {
            // Get all values in Difficulty Level
            foreach (DifficultyLevel level in Enum.GetValues(typeof(DifficultyLevel)))
            {
                // If the enum value name is equals to the input, return input.
                if (level.ToString().Equals(input))
                {
                    return level;
                }
            }
            // If doesn't match, return Easy.
            return DifficultyLevel.Easy;
        }

    }

}