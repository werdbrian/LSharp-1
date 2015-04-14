#region

using System;
using LeagueSharp;
using LeagueSharp.Common;


#endregion

namespace Mata_View
{
    internal class Program
    {
       
        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Chat("1.1.2.4 [Mar, 24]");
            Menus.Menuadd();
            DetectObj.DetectObjload();
        }

        private static void Chat(string message)
        {
            Game.PrintChat("<font color=\"#4EE2EC\">Mata View</font> - " + message + " - <font color=\"#B6B6B4\">E2Slayer</font>");
        }

    }
}
