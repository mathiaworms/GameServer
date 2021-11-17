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
    public class GravesSmokeGrenade : ISpellScript
    {
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
        public ISpellSector SmokeSector;
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
            var targetPos = GetPointFromUnit(owner, 950.0f);
            SpellCast(owner, 1, SpellSlotType.ExtraSlots, targetPos, targetPos, false, Vector2.Zero);
            var spellpos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);


                AddParticle(owner, null, "Graves_SmokeGrenade_Boom.troy", spellpos, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "Graves_SmokeGrenade_Cas.troy", spellpos, lifetime: 0.5f , reqVision: false);
                 AddParticle(owner, null, "Graves_SmokeGrenade_mis.troy", spellpos, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "Graves_SmokeGrenade_Cloud_Team_Green.troy", spellpos, lifetime: 4f , reqVision: false);
                AddParticle(owner, null, "Graves_SmokeGrenade_Cloud_Team_Red.troy", spellpos, lifetime: 4f , reqVision: false);
                DamageSector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 250f,
                    Tickrate = 2,
                    CanHitSameTargetConsecutively = false,
                    OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area,
                    Lifetime = 4f
                });
                SmokeSector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 250f,
                    Tickrate = 2,
                    CanHitSameTargetConsecutively = true,
                    OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area,
                    Lifetime = 4f
                });

        }

        public void OnSpellChannel(ISpell spell)
        {
        }
          public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {  
             if (SmokeSector != null && sector == SmokeSector)
            {
                AddBuff("GravesSmokeGrenade", 1.0f, 1, spell, target, spell.CastInfo.Owner);

                return;
            }

                 var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.6f;
                 var damage = 10 + (50 * spell.CastInfo.SpellLevel ) + ap;

                target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
            
            

            
           

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
