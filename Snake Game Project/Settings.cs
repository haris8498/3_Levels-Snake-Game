using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Game_Project
{
    public enum Directions
    {
        Left,
        Right,
        Up,
        Down
    };
    internal class Settings
    {
        public static int Width { get; set; } //This is a public static int class called Width

        public static int Height { get; set; } //This is a public static int class called Height

        public static int Speed { get; set; } //This is a public static int class called Speed

        public static int Score { get; set; } //This is a public static int class called Score

        public static int Points { get; set; } //This is a public static int class called Points

        public static bool GameOver { get; set; } //This is a public static int class called GameOver

        public static Directions direction { get; set; } //This is a public static Directions class called direction

        public Settings() //This is a public settings class
        {
            Width = 16; //This sets the Width value to 16
            Height = 16; //This sets the Height value to 16
            Speed = 10; //This sets the Speed value to 2
            Score = 0; //This sets the Score value to 0
            Points = 100; //This sets the Points value to 100
            GameOver = false; //This sets the GameOver value to false
            direction = Directions.Down; //This sets the direction value to Down
        }
    }
}
