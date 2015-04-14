#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;


#endregion

namespace Mata_View
{
    internal class DetectObj
    {
        private static readonly List<ListedText> AllSkills = new List<ListedText>();


        private static bool _panthD, _kayleUlt;
        private static bool _revive = false;
        private static float _kayleDuration;
        public static Obj_AI_Hero Heropos = new Obj_AI_Hero();


        public static void DetectObjload()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;

            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
        }

        private static void OnProcessSpell(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
            try { 
            if (obj == null || arg == null || obj.Name.Contains("Turret") || obj.Name.Contains("Minion") ||
                !Menus.Menu.Item("activeskill").GetValue<bool>())
                return;

            FakeObjCreate(obj, arg);
            }
            catch (Exception)
            {
                //semmi
            }

        }

        private static void FakeObjCreate(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
            try { 
            if (arg.SData.Name.Contains("JudicatorIntervention"))
            {
                if (Menus.Menu.Item("eyeforaneye").GetValue<bool>())
                {
                    if (obj.Spellbook.GetSpell(SpellSlot.R).Level == 0)
                        return;
                    var level = obj.Spellbook.GetSpell(SpellSlot.R).Level;
                    _kayleDuration = 2f + (level - 1) * 0.5f;
                    _kayleUlt = true;
                }
            }
            }
            catch (Exception)
            {
                //semmi
            }
            try { 
            if (arg.SData.Name.Contains("ZhonyasHourglass"))
            {
                if (Menus.Menu.Item("activeMisc").GetValue<bool>())
                {
                    FakeCreateLogic(obj, "zhonyas_ring_activate.troy", 999999998, false, "zhonyas_ring_activate.troy");
                }
            }
            }
            catch (Exception)
            {
                //semmi
            }
            try
            { 
            if (arg.SData.Name.Contains("KarthusFallenOne"))
            {
                FakeCreateLogic(obj, "Karthus_Base_R_Cas.troy", 999999997, false, "Karthus_Base_R_Cas.troy");
            }
            }
            catch (Exception)
            {
                //semmi
            }
            try { 
            if (arg.SData.Name.Contains("infiniteduresschannel"))
            {
                FakeCreateLogic(obj, "InfiniteDuress_tar.troy", 999999996, true, "InfiniteDuress_");
            }
            }
            catch (Exception)
            {
                //semmi
            }
            try
            { 
            if (arg.SData.Name.Contains("Sadism"))
            {
                FakeCreateLogic(obj, "dr_mundo_heal.troy", 999999995, false, "dr_mundo_heal.troy");
            }
            }
            catch (Exception)
            {
                //semmi
            }
            try { 
            if (arg.SData.Name.Contains("PantheonRJump"))
                _panthD = true;
            }
            catch (Exception)
            {
                //semmi
            }
            /*
            if (arg.SData.Name.Contains("TalonShadowAssault"))
            {
               FakeCreateLogic(obj, "talon_ult_sound.troy", 999999996, false, "talon_ult_sound.troy");
            }*/
            // talon is kind of buggy



        }

        public static void FakeCreateLogic(Obj_AI_Base obj,
            string createname,
            int fakeNetId,
            bool namecheck, 
            string changname)
        {
            if (!Menus.Menu.Item(createname).GetValue<bool>())
                return;
            var ho = SkillsList.IsObj(createname);
            if (ho == null)
                return;

            var misccheck = SkillsList.IsMisc(createname);
            if (misccheck != null)
                return;

            if (namecheck)
                createname = changname;

            var sender = new GameObject();
            try { 
            foreach (var hero in
                ObjectManager.Get<Obj_AI_Hero>().Where(d => obj.BaseSkinName == d.BaseSkinName))
            {
                AllSkills.Add(
                    new ListedText(
                        fakeNetId, createname, ho.Duration, hero.Position, Game.Time, sender, ho.Realtime, hero));
            }
            }
            catch (Exception)
            {
                //semmi
            }
        }

    
        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            try { 

            // obj_AI_Minion can't type to "ObjectManager.GetUnitByNetworkId<Obj_GeneralParticleEmitter>", so it throw error.
            if (sender == null || args == null || sender.Type != GameObjectType.obj_GeneralParticleEmmiter ||
                !Menus.Menu.Item("activeskill").GetValue<bool>())
                return;
            if (sender.Name.Contains("missile") || sender.Name.Contains("Minion") ||
                sender.Name.Contains("InfiniteDuress_tar") || sender.Name.Contains("teleport_arrive") ||
                sender.Name.Contains("teleport_flash"))
                return;

            var ho =
                SkillsList.IsObj((ObjectManager.GetUnitByNetworkId<Obj_GeneralParticleEmitter>(sender.NetworkId)).Name);
            if (ho == null)
                return;

            var misccheck =
                SkillsList.IsMisc((ObjectManager.GetUnitByNetworkId<Obj_GeneralParticleEmitter>(sender.NetworkId)).Name);
            if (misccheck != null)
                return;
            if (sender.Name == "nickoftime_tar.troy")
                _revive = true;
            {
                switch (ho.Realtime)
                {
                    case 0: //Object First Created Posistion
                        var r1 = DivideRealTime.RealTimeDivide(sender, 1);
                        if (r1 == null)
                            return;
                        if (_panthD)
                        {
                            ho.Duration = 2.5f;
                            _panthD = false;
                        }
                        break;
                    case 1: //Realtime on Hero Position / Don't need to check Hero.isenemy
                        var r2 = DivideRealTime.RealTimeDivide(sender, 2);
                        if (r2 == null)
                            return;
                        if (_kayleUlt)
                        {
                            if (!Menus.Menu.Item("eyeforaneye").GetValue<bool>())
                                return;
                            ho.Duration = _kayleDuration;
                            _kayleUlt = false;
                        }
                        break;
                    case 2: //Realtime on Sender Position
                        var r3 = DivideRealTime.RealTimeDivide(sender, 1);
                        if (r3 == null)
                            return;
                        if (_revive)
                        {
                            ho.Duration = 3.0f;
                            _revive = false;
                        }
                        else if (_revive == false && sender.Name == "LifeAura.troy")
                            ho.Duration = 4.0f;
                        break;

                }
                AllSkills.Add(
                    new ListedText(
                        sender.NetworkId, sender.Name, ho.Duration, sender.Position, Game.Time, sender, ho.Realtime,
                        Heropos));
            }
            }
            catch (Exception)
            {
                //semmi
            }
        }






        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            try { 
            if (sender == null || args == null || sender.Type != GameObjectType.obj_GeneralParticleEmmiter ||
                sender.Name.Contains("missile") || sender.Name.Contains("Minion") ||
                !Menus.Menu.Item("activeskill").GetValue<bool>())
                return;

            //InvalidOperationException is fixed 
                try
                {
            foreach (
                var deleteobj in
                    new List<ListedText>(AllSkills).Where(w => w.NetworkId == sender.NetworkId || w.Name == sender.Name)
                )
            {
                deleteobj.Visible = false;
                AllSkills.Remove(deleteobj);
            }
                }
                catch (Exception)
                {
                    //semmi
                }
            }
            catch (Exception)
            {
                //semmi
            }
        }

    }

}

