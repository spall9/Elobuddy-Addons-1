﻿using System;
using System.Linq;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace UBRyze
{
    static class Extensions
    {
        public static string AddonName = "UBRyze";
        public static int GetValue(this Menu menu, string id, bool IsSlider = true)
        {
            if (IsSlider)
                return menu[id].Cast<Slider>().CurrentValue;
            else
                return menu[id].Cast<ComboBox>().CurrentValue;
        }
        public static bool Checked(this Menu menu, string id, bool IsCheckBox = true)
        {
            if (IsCheckBox)
                return menu[id].Cast<CheckBox>().CurrentValue;
            else
                return menu[id].Cast<KeyBind>().CurrentValue;
        }
        public static bool Unkillable(AIHeroClient target)
        {
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "UndyingRage"))
            {
                return true;
            }
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "ChronoShift"))
            {
                return true;
            }
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "kindredrnodeathbuff"))
            {
                return true;
            }
            if (target.HasBuffOfType(BuffType.Invulnerability))
            {
                return true;
            }
            return target.IsInvulnerable;
        }
        public static int CountminionEbuff
        {
            get { return EntityManager.MinionsAndMonsters.EnemyMinions.Count(m => m.HasBuff("RyzeE")); }
        }
        public static Obj_AI_Minion MinionHasEBuff
        {
            get { return EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.HasBuff("RyzeE") && m.IsValid && Spells.E.IsInRange(m)).OrderBy(m => m.Health).FirstOrDefault(); }
        }
        public static Obj_AI_Minion MinionEDie
        {
            get { return EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.Health <= Damages.EDamage(m) && m.IsValid && Spells.E.IsInRange(m)).OrderBy(m => m.MaxHealth).FirstOrDefault(); }
        }
        public static bool CanNextSpell
        {
            get
            {
                var Delay = Config.Menu.GetValue("style") == 0 ? Config.Menu.GetValue("delay1") : new Random().Next(Config.Menu.GetValue("delay1") * 10, Config.Menu.GetValue("delay2") * 10);
                return !Config.Menu.Checked("human") || Extensions.LastCast * 1000 + Delay <= Game.Time * 1000;
            }
        } 
        public static float LastCast;
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var Target = TargetSelector.GetTarget(Spells.W.Range, DamageType.Magical);
            if (!sender.IsMe) return;
            if ((new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E }).Contains(args.Slot))
            {
                LastCast = Game.Time;
            }
            if (args.SData.Name.Contains("SummonerFlash"))
            {
                if (Target != null)
                {
                    switch (Config.AutoMenu["autofl"].Cast<ComboBox>().CurrentValue)
                    {
                        case 0:
                            break;
                        case 1:
                            {
                                if(Spells.W.IsReady())
                                Spells.W.Cast(Target);
                            }
                            break;
                        case 2:
                            {
                                if (Spells.E.IsReady())
                                    Spells.E.Cast(Target);
                                if (Spells.W.IsReady() && Target.HasBuff("RyzeE"))
                                    Spells.W.Cast(Target);
                            }
                            break;
                    }
                }
            }
        }
        //public static Dictionary<string, float> RemainingTime = new Dictionary<string, float>
        //{          
        //    {"QStacks", 0}            
        //};
        //public static Dictionary<string, float> TimeStore = new Dictionary<string, float>
        //{            
        //    {"Qstacks", 0}
        //};
        //public static byte RyzeQStack { get; private set; }
        //public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        //{
        //    if (!sender.IsMe) return;
        //    if (args.SData.Name.Contains("RyzeW") || args.SData.Name.Contains("RyzeE") || args.SData.Name.Contains("RyzeR"))
        //    {
        //        if (RyzeQStack < 2)
        //        RyzeQStack++;
        //        TimeStore["QStacks"] = Game.Time + 4f;
        //    }
        //    if (args.SData.Name.Contains("RyzeQ"))
        //    {
        //        RyzeQStack = 0;
        //    }
        //    RyzeQIconFullCharge
        //        RyzeQIconNoCharge
        //}
        //public static void SpellsOnUpdate(EventArgs args)
        //{
        //    RemainingTime["QStacks"] = ((TimeStore["QStacks"] - Game.Time) > 0)
        //        ? (TimeStore["QStacks"] - Game.Time)
        //        : 0;
        //    if (RemainingTime["QStacks"] == 0)
        //    {
        //        RyzeQStack = 0;
        //    }
        //}
    }
}
