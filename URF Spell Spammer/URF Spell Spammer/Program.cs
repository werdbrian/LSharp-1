using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace URF_Spell_Spammer
{
    internal class Program
    {
        public static Menu Menu;
        public static Obj_AI_Hero Player = ObjectManager.Player;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("URF Spell Spammer Loaded");
            Menu = new Menu("URF Spell Spammer", "URF Spell Spammer", true);
            Menu.AddItem(new MenuItem("Q", "Q").SetValue(false));
            Menu.AddItem(new MenuItem("W", "W").SetValue(false));
            Menu.AddItem(new MenuItem("E", "E").SetValue(false));
            Menu.AddToMainMenu();
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Menu.Item("Q").GetValue<bool>())
                {
                    if (Player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready &&
                        Player.Spellbook.GetSpell(SpellSlot.Q).Level > 0)
                    {
                        Player.Spellbook.CastSpell(SpellSlot.Q);
                    }
                }
                if (Menu.Item("W").GetValue<bool>())
                {
                    if (Player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready &&
                        Player.Spellbook.GetSpell(SpellSlot.W).Level > 0)
                    {
                        Player.Spellbook.CastSpell(SpellSlot.W);
                    }
                }
                if (Menu.Item("E").GetValue<bool>())
                {
                    if (Player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready &&
                        Player.Spellbook.GetSpell(SpellSlot.E).Level > 0)
                    {
                        Player.Spellbook.CastSpell(SpellSlot.E);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}