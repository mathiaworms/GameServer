using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Missile;

namespace Buffs
{
    class AuraofDespair : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;
      
        float DamageManaTimer;
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        ISpell originSpell;
        float[] manaCost = { 8.0f , 8.0f , 8.0f , 8.0f , 8.0f };
        IObjAiBase Owner;
        IBuff thisBuff;
        public ISpellSector AuraAmumu;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Owner = ownerSpell.CastInfo.Owner;
            originSpell = ownerSpell;
            thisBuff = buff;
            ApiEventManager.OnSpellHit.AddListener(this, ownerSpell, TargetExecute, false);



            AuraAmumu = ownerSpell.CreateSpellSector(new SectorParameters
            {
                BindObject = ownerSpell.CastInfo.Owner,
                Length = 300f,
                Tickrate = 1,
                CanHitSameTargetConsecutively = true,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                Type = SectorType.Area
            });
        }
        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {   
             float ap = Owner.Stats.AbilityPower.Total ;
             float lvlmaxhp = ( ((  0.25f * (ap/100) ) + ( 0.425f + 0.075f *  spell.CastInfo.SpellLevel )) * target.Stats.HealthPoints.Total ) /100; 
             var damage = 4 + spell.CastInfo.SpellLevel * 2 + lvlmaxhp;
           

            target.TakeDamage(Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL,DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
        }
        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            ApiEventManager.OnSpellHit.RemoveListener(this);
            AuraAmumu.SetToRemove();
        }
        public void OnUpdate(float diff)
        {
             if (Owner != null && thisBuff != null && originSpell != null)
            {
                DamageManaTimer += diff;

                if (DamageManaTimer >= 500f)
                {
                    if (manaCost[originSpell.CastInfo.SpellLevel - 1] > Owner.Stats.CurrentMana)
                    {
                        RemoveBuff(thisBuff);
                    }
                    else
                    {
                        Owner.Stats.CurrentMana -= manaCost[originSpell.CastInfo.SpellLevel - 1];
                    }

                    DamageManaTimer = 0;
                }

                
            }

        }
    }
}
