using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Snake_Game_Project
{
    public static class Input
    {
        private static Dictionary<Keys, bool> keyTable = new Dictionary<Keys, bool>();

        public static void ChangeState(Keys key, bool state)
        {
            keyTable[key] = state;
        }

        public static bool KeyPress(Keys key)
        {
            bool state = false;
            if (keyTable.ContainsKey(key))
            {
                state = keyTable[key];
                keyTable[key] = false;
            }
            return state;
        }
    }
}