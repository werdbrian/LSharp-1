#region

using System;
using System.Linq;

using LeagueSharp;
using SharpDX;

#endregion

namespace Mata_View
{
    public class DivideRealTime
    {

        public static object RealTimeDivide(GameObject sender, int realtimecheck)
        {
                var herof = new Obj_AI_Hero();
                switch (realtimecheck)
                {
                    case 1:
                        try
                        {
                        foreach (
                            var hero in
                                ObjectManager.Get<Obj_AI_Hero>()
                                    .Where(
                                        hero =>
                                            (Vector3.Distance(sender.Position, hero.ServerPosition) <= 1000) &&
                                            sender.Name.ToLower().Contains(hero.ChampionName.ToLower())))
                        {
                            herof = hero;
                            break;
                        }
                        }
                        catch (Exception)
                        {
                            //semmi
                        }
                        break;
                    case 2:
                        try
                        {
                        foreach (
                            var hero in
                                ObjectManager.Get<Obj_AI_Hero>()
                                    .Where(o => (Vector3.Distance(sender.Position, o.ServerPosition) <= 100)))
                        {
                            herof = hero;
                            break;
                        }
                        }
                        catch (Exception)
                        {
                            //semmi
                        }
                        break;
                    // return null;

        }
                try { 
            foreach (var skill in SkillsList.SkillList0.Where(o => sender.Name.ToLower().Contains(o.Name.ToLower())))
            {
                try
                {
                if (herof.ChampionName == "Lissandra" &&
                    sender.Name.ToLower().Contains("lissandra_base_r_ring_"))
                    return null;
                if (!Menus.Menu.Item(skill.Name).GetValue<bool>()) return null;
                if ((herof.IsEnemy || (sender.Name.ToLower().Contains("red") || sender.Name.ToLower().Contains("enemy"))) &&
                    Menus.Menu.Item("activeEnemy").GetValue<bool>())
                {
                    DetectObj.Heropos = herof;
                    return skill;
                }
                if ((herof.IsAlly || (sender.Name.ToLower().Contains("green") || sender.Name.ToLower().Contains("blue"))) &&
                    !herof.IsMe && Menus.Menu.Item("activeEnemy").GetValue<bool>())
                {
                    DetectObj.Heropos = herof;
                    return skill;
                }
                if ((herof.IsMe || sender.Name.ToLower().Contains("green")) &&
                    Menus.Menu.Item("activeMy").GetValue<bool>())
                {
                    DetectObj.Heropos = herof;
                    return skill;
                }
                }
                catch (Exception)
                {
                    //semmi
                }
                return null;
            }
                }
                catch (Exception)
                {
                    //semmi
                }
            return null;
        }
    }



}