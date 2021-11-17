using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Missile;


namespace Spells
{
    public class AuraofDespair : ISpellScript
    {
        IBuff thisBuff;
        public ISpellSector DamageSector;

         public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            CastingBreaksStealth = true,
            DoesntBreakShields = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true,
            SpellToggleSlot = 4
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {

        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            
        }

        public void OnSpellCast(ISpell spell)
        {
           
        }
       
      public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;

            if (owner.HasBuff("AuraofDespair"))
            {
                owner.RemoveBuffsWithName("AuraofDespair");

            }
            else
            {
                thisBuff = AddBuff("AuraofDespair", 25000.0f, 1, spell, owner, owner);
                
                DamageSector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 300f,
                    Tickrate = 2,
                    CanHitSameTargetConsecutively = true,
                    OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area
                });
           
                
            }
        }
        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
           
                if (!spell.CastInfo.Owner.HasBuff(thisBuff))
                {
                   return;
                }
            ApplyAuraDamage(owner, spell, target);


        }
         private void ApplyAuraDamage(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            
            var units = GetUnitsInRange(owner.Position, 300, true);
            foreach (var unit in units)
            {
                if (unit.Team != owner.Team)
                {
                   var ap = owner.Stats.AbilityPower.Total;
                   var lvlmaxhp =  (( 0.375f + 0.125f * spell.CastInfo.SpellLevel + 0.25f * ap ) * unit.Stats.HealthPoints.Total )/100 ; 
                   var damage = 4 + spell.CastInfo.SpellLevel * 2 + lvlmaxhp;
                   
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE,
                        false);
                }
            }
        }
        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
            


          
        }
    }
}

