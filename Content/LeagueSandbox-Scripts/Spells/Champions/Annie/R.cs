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
using GameServerCore;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Enums;
using System.Numerics;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class InfernalGuardian : ISpellScript
    {
        public float petTimeAlive = 0.00f;
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            CastingBreaksStealth = true,
            DoesntBreakShields = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true,


        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
             ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public ISpellSector DamageSector;

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
            var targetPos = GetPointFromUnit(owner, 600.0f);
            SpellCast(owner, 1, SpellSlotType.ExtraSlots, targetPos, targetPos, false, Vector2.Zero);
            var spellpos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);        
            var castrange = spell.GetCurrentCastRange();

                AddParticle(owner, null, "infernalguardian_tar.troy", spellpos, lifetime: 0.5f , reqVision: false);
                
                DamageSector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 350f,
                    Tickrate = 2,
                    CanHitSameTargetConsecutively = false,
                    OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area,
                    Lifetime = 0.5f
                });
              var tibbersduration = 45.0f;

             if (Extensions.IsVectorWithinRange(owner.Position, spellpos, castrange))
                     {
                       IMinion m = AddMinion((IChampion)owner, "AnnieTibbers", "AnnieTibbers", spellpos);
                     //  m.IsPet = true ; 
                     //  AddBuff("AniviaIceBlock", 5f, 1, spell, m, owner);
                     //    AddParticle(owner, null, "JackintheboxPoof.troy", spellPos);

                      //  var attackrange = m.Stats.Range.Total;

                
                    if (!m.IsDead)
                    {
                        CreateTimer(tibbersduration, () =>
                        {
                            if (!m.IsDead)
                            {
                                //TODO: Fix targeting issues
                                m.TakeDamage(m.Owner, 10000f, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, DamageResultType.RESULT_NORMAL);
                            }
                        });
                    }
                
            }
                

        }

        public void OnSpellChannel(ISpell spell)
        {
        }
          public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {  
            var owner = spell.CastInfo.Owner;
                 var stunduration = 1.25f + 0.25f * owner.GetSpell("InfernalGuardian").CastInfo.SpellLevel;
                 var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.75f;
                var damage = 25.0f + (125.0f * spell.CastInfo.SpellLevel ) + ap;
                target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                
            if( spell.CastInfo.Owner.HasBuff("Pyromania"))
            {  

                if (owner.GetBuffWithName("Pyromania").StackCount == 4)
                  {
                      AddBuff("Stun", stunduration, 1, spell, target, owner);
                    owner.RemoveBuffsWithName("Pyromania");
                   }
                   else {
                    AddBuff("Pyromania", 25000f, 1, spell, owner, owner);
                   }
            }
            else {
                 AddBuff("Pyromania", 25000f, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner);
             
            }

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
