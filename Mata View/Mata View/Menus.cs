#region

using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace Mata_View
{
    public class Menus
    {
        public static Menu Menu, Skill;
        public static MenuItem Testsize;
        public static SkillsList SkillsListAdd;

        public static void Menuadd()
        {
            Menu = new Menu("Mata View", "Mata View", true);
            var configs = (new Menu("Skills Timer Configs", "Skills Timer Configs"));
            var activeskill = (new MenuItem("activeskill", "Active All Skill Timer").SetValue(true));

            var textmenu = (new Menu("Text Configs", "Text Configs"));
            Testsize = (new MenuItem("textsize", "Text Size").SetValue(new Slider(16, 30, 10)));
            var enemylist = new Menu("Enemy Timer", "enemylist");
            var allylist = new Menu("Ally Timer", "allylist");
            var mylist = new Menu("My Timer", "mylist");
            var misclist = new Menu("Misc Timer", "misclist");
       

            Menu.AddSubMenu(configs);
            Menu.SubMenu("Skills Timer Configs").AddItem(activeskill);

            Menu.AddSubMenu(textmenu);
            Menu.SubMenu("Text Configs").AddItem(Testsize);
  
            Skill = new Menu("Skill Timer", "Skill Timer", true);
            Menu.AddSubMenu(enemylist);
            Menu.AddSubMenu(allylist);
            Menu.AddSubMenu(mylist);
            Menu.AddSubMenu(misclist);
  
            foreach (var skill in SkillsList.SkillList0)
            {
                foreach (var herolist in ObjectManager.Get<Obj_AI_Hero>())
                {
                    if (herolist.IsEnemy && skill.Champname == herolist.ChampionName)
                    {
                        Menu.SubMenu("enemylist").AddItem(new MenuItem(skill.Name, skill.Displayname).SetValue(true));
                    }
                    else if (herolist.IsAlly && !herolist.IsMe && skill.Champname == herolist.ChampionName)
                    {
                        Menu.SubMenu("allylist").AddItem(new MenuItem(skill.Name, skill.Displayname).SetValue(true));
                    }
                    else if (herolist.IsMe && skill.Champname == herolist.ChampionName)
                    {
                        Menu.SubMenu("mylist").AddItem(new MenuItem(skill.Name, skill.Displayname).SetValue(true));
                    }
                }
               
                   if (skill.Champname == "Misc")
                    {
                        Menu.SubMenu("misclist").AddItem(new MenuItem(skill.Name, skill.Displayname).SetValue(true));
                    }
            }
            Menu.SubMenu("enemylist").AddItem(new MenuItem("activeEnemy", "Active Enemy SKills Timer").SetValue(true));
            Menu.SubMenu("allylist").AddItem(new MenuItem("activeAlly", "Active Ally SKills Timer").SetValue(true));
            Menu.SubMenu("mylist").AddItem(new MenuItem("activeMy", "Active My SKills Timer").SetValue(true));
            Menu.SubMenu("misclist").AddItem(new MenuItem("activeMisc", "Active Misc SKills Timer").SetValue(true));
            Menu.AddToMainMenu();
        }
    }
}
