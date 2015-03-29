using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Orbwalk = MasterSeries.Common.M_Orbwalker;

namespace MasterSeries.Champions
{
    class Amumu : Program
    {
        public Amumu()
        {
            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 300);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 550);
            Q.SetSkillshot(-0.5f, 80, 2000, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(-0.3864f, 0, 0, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(-0.5f, 0, 0, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(-0.5f, 0, 20, false, SkillshotType.SkillshotCircle);

            var ChampMenu = new Menu("Plugin", Name + "Plugin");
            {
                var ComboMenu = new Menu("Combo", "Combo");
                {
                    ItemBool(ComboMenu, "Q", "Use Q");
                    ItemBool(ComboMenu, "W", "Use W");
                    ItemSlider(ComboMenu, "WAbove", "-> If Mp Above", 20);
                    ItemBool(ComboMenu, "E", "Use E");
                    ItemBool(ComboMenu, "R", "Use R");
                    ItemList(ComboMenu, "RMode", "-> Mode", new[] { "Killable", "# Enemy" });
                    ItemSlider(ComboMenu, "RAbove", "--> If Enemy Above", 2, 1, 4);
                    ItemBool(ComboMenu, "Item", "Use Item");
                    ItemBool(ComboMenu, "Ignite", "Auto Ignite If Killable");
                    ChampMenu.AddSubMenu(ComboMenu);
                }
                var HarassMenu = new Menu("Harass", "Harass");
                {
                    ItemBool(HarassMenu, "W", "Use W");
                    ItemSlider(HarassMenu, "WAbove", "-> If Mp Above", 20);
                    ItemBool(HarassMenu, "E", "Use E");
                    ChampMenu.AddSubMenu(HarassMenu);
                }
                var ClearMenu = new Menu("Lane/Jungle Clear", "Clear");
                {
                    var SmiteMob = new Menu("Smite Mob If Killable", "SmiteMob");
                    {
                        ItemBool(SmiteMob, "Baron", "Baron Nashor");
                        ItemBool(SmiteMob, "Dragon", "Dragon");
                        ItemBool(SmiteMob, "Red", "Red Brambleback");
                        ItemBool(SmiteMob, "Blue", "Blue Sentinel");
                        ItemBool(SmiteMob, "Krug", "Ancient Krug");
                        ItemBool(SmiteMob, "Gromp", "Gromp");
                        ItemBool(SmiteMob, "Raptor", "Crimson Raptor");
                        ItemBool(SmiteMob, "Wolf", "Greater Murk Wolf");
                        ClearMenu.AddSubMenu(SmiteMob);
                    }
                    ItemBool(ClearMenu, "Q", "Use Q");
                    ItemBool(ClearMenu, "W", "Use W");
                    ItemSlider(ClearMenu, "WAbove", "-> If Mp Above", 20);
                    ItemBool(ClearMenu, "E", "Use E");
                    ChampMenu.AddSubMenu(ClearMenu);
                }
                var MiscMenu = new Menu("Misc", "Misc");
                {
                    ItemBool(MiscMenu, "QAntiGap", "Use Q To Anti Gap Closer");
                    ItemBool(MiscMenu, "SmiteCol", "Auto Smite Collision");
                    ItemSlider(MiscMenu, "CustomSkin", "Skin Changer", 6, 0, 7).ValueChanged += SkinChanger;
                    ChampMenu.AddSubMenu(MiscMenu);
                }
                var DrawMenu = new Menu("Draw", "Draw");
                {
                    ItemBool(DrawMenu, "Q", "Q Range", false);
                    ItemBool(DrawMenu, "W", "W Range", false);
                    ItemBool(DrawMenu, "E", "E Range", false);
                    ItemBool(DrawMenu, "R", "R Range", false);
                    ChampMenu.AddSubMenu(DrawMenu);
                }
                Config.AddSubMenu(ChampMenu);
            }
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsChannelingImportantSpell() || Player.IsRecalling()) return;
            if (Orbwalk.CurrentMode == Orbwalk.Mode.Combo || Orbwalk.CurrentMode == Orbwalk.Mode.Harass)
            {
                NormalCombo(Orbwalk.CurrentMode.ToString());
            }
            else if (Orbwalk.CurrentMode == Orbwalk.Mode.LaneClear || Orbwalk.CurrentMode == Orbwalk.Mode.LaneFreeze) LaneJungClear();
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (ItemBool("Draw", "Q") && Q.Level > 0) Utility.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);
            if (ItemBool("Draw", "W") && W.Level > 0) Utility.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);
            if (ItemBool("Draw", "E") && E.Level > 0) Utility.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);
            if (ItemBool("Draw", "R") && R.Level > 0) Utility.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!ItemBool("Misc", "QAntiGap") || Player.IsDead || !Q.IsReady()) return;
            if (IsValid(gapcloser.Sender, Q.Range) && Player.Distance3D(gapcloser.Sender) < 400) Q.Cast(gapcloser.Sender.Position, PacketCast());
        }

        private void NormalCombo(string Mode)
        {
            if (ItemBool(Mode, "W") && W.IsReady() && Player.HasBuff("AuraofDespair") && Player.CountEnemysInRange(500) == 0) W.Cast(PacketCast());
            if (targetObj == null) return;
            if (Mode == "Combo" && ItemBool(Mode, "Q") && Q.IsReady())
            {
                var nearObj = ObjectManager.Get<Obj_AI_Base>().Where(i => IsValid(i, Q.Range) && !(i is Obj_AI_Turret) && i.CountEnemysInRange((int)R.Range - 40) >= ItemSlider(Mode, "RAbove") && !CanKill(i, Q)).OrderBy(i => i.CountEnemysInRange((int)R.Range));
                if (ItemBool(Mode, "R") && R.IsReady() && ItemList(Mode, "RMode") == 1 && nearObj.Count() > 0)
                {
                    foreach (var Obj in nearObj) Q.CastIfHitchanceEquals(Obj, HitChance.VeryHigh, PacketCast());
                }
                else if (Q.IsInRange(targetObj.Position) && (CanKill(targetObj, Q) || !Orbwalk.InAutoAttackRange(targetObj)))
                {
                    if (ItemBool("Misc", "SmiteCol"))
                    {
                        if (!SmiteCollision(targetObj, Q)) Q.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast());
                    }
                    else Q.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast());
                }
            }
            if (ItemBool(Mode, "W") && W.IsReady())
            {
                if (Player.ManaPercentage() >= ItemSlider(Mode, "WAbove"))
                {
                    if (Player.Distance3D(targetObj) <= W.Range + 60)
                    {
                        if (!Player.HasBuff("AuraofDespair")) W.Cast(PacketCast());
                    }
                    else if (Player.HasBuff("AuraofDespair")) W.Cast(PacketCast());
                }
                else if (Player.HasBuff("AuraofDespair")) W.Cast(PacketCast());
            }
            if (ItemBool(Mode, "E") && E.IsReady() && E.IsInRange(targetObj.Position)) E.Cast(PacketCast());
            if (Mode == "Combo" && ItemBool(Mode, "R") && R.IsReady())
            {
                switch (ItemList(Mode, "RMode"))
                {
                    case 0:
                        if (R.IsInRange(targetObj.Position) && CanKill(targetObj, R)) R.Cast(PacketCast());
                        break;
                    case 1:
                        var Obj = ObjectManager.Get<Obj_AI_Hero>().Where(i => IsValid(i, R.Range));
                        if (Obj.Count() > 0 && (Obj.Count() >= ItemSlider(Mode, "RAbove") || (Obj.Count() >= 2 && Obj.Count(i => CanKill(i, R)) >= 1))) R.Cast(PacketCast());
                        break;
                }
            }
            if (Mode == "Combo" && ItemBool(Mode, "Item") && Items.CanUseItem(Randuin) && Player.CountEnemysInRange(450) >= 1) Items.UseItem(Randuin);
            if (Mode == "Combo" && ItemBool(Mode, "Ignite") && IgniteReady()) CastIgnite(targetObj);
        }

        private void LaneJungClear()
        {
            var minionObj = ObjectManager.Get<Obj_AI_Minion>().Where(i => IsValid(i, Q.Range)).OrderBy(i => i.Health);
            if (minionObj.Count() == 0 && ItemBool("Clear", "W") && W.IsReady() && Player.HasBuff("AuraofDespair")) W.Cast(PacketCast());
            foreach (var Obj in minionObj)
            {
                if (SmiteReady() && Obj.Team == GameObjectTeam.Neutral)
                {
                    if ((ItemBool("SmiteMob", "Baron") && Obj.Name.StartsWith("SRU_Baron")) || (ItemBool("SmiteMob", "Dragon") && Obj.Name.StartsWith("SRU_Dragon")) || (!Obj.Name.Contains("Mini") && (
                        (ItemBool("SmiteMob", "Red") && Obj.Name.StartsWith("SRU_Red")) || (ItemBool("SmiteMob", "Blue") && Obj.Name.StartsWith("SRU_Blue")) ||
                        (ItemBool("SmiteMob", "Krug") && Obj.Name.StartsWith("SRU_Krug")) || (ItemBool("SmiteMob", "Gromp") && Obj.Name.StartsWith("SRU_Gromp")) ||
                        (ItemBool("SmiteMob", "Raptor") && Obj.Name.StartsWith("SRU_Razorbeak")) || (ItemBool("SmiteMob", "Wolf") && Obj.Name.StartsWith("SRU_Murkwolf"))))) CastSmite(Obj);
                }
                if (ItemBool("Clear", "E") && E.IsReady() && E.IsInRange(Obj.Position)) E.Cast(PacketCast());
                if (ItemBool("Clear", "W") && W.IsReady())
                {
                    if (Player.ManaPercentage() >= ItemSlider("Clear", "WAbove"))
                    {
                        if (minionObj.Count(i => Player.Distance3D(i) <= W.Range + 60) >= 2 || (Obj.MaxHealth >= 1200 && Player.Distance3D(Obj) <= W.Range + 60))
                        {
                            if (!Player.HasBuff("AuraofDespair")) W.Cast(PacketCast());
                        }
                        else if (Player.HasBuff("AuraofDespair")) W.Cast(PacketCast());
                    }
                    else if (Player.HasBuff("AuraofDespair")) W.Cast(PacketCast());
                }
                if (ItemBool("Clear", "Q") && Q.IsReady() && (!Orbwalk.InAutoAttackRange(Obj) || CanKill(Obj, Q))) Q.CastIfHitchanceEquals(Obj, HitChance.Medium, PacketCast());
            }
        }
    }
}