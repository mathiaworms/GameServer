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
    public class KarthusLayWasteA1  : ISpellScript
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
            var targetPos = GetPointFromUnit(owner, 875.0f);
            SpellCast(owner, 1, SpellSlotType.ExtraSlots, targetPos, targetPos, false, Vector2.Zero);
            var spellpos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);        


                AddParticle(owner, null, "Karthus_Base_Q_Point.troy", spellpos, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "Karthus_Base_Q_Point_red.troy", spellpos, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "Karthus_Base_Q_Explosion.troy", spellpos, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "Karthus_Base_Q_Explosion_Sound.troy", spellpos, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "Karthus_Base_Q_Tar.troy", spellpos, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "Karthus_Base_Q_Tar_red.troy", spellpos, lifetime: 0.5f , reqVision: false);
                DamageSector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 160f,
                    Tickrate = 2,
                    CanHitSameTargetConsecutively = false,
                    OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area,
                    Lifetime = 0.5f
                });

        }

        public void OnSpellChannel(ISpell spell)
        {
        }
          public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {  
            var owner = spell.CastInfo.Owner;

            var coords = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);
            var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.35f;
            var damage = 27.5f + (17.5f * spell.CastInfo.SpellLevel ) + ap;
             target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);

             var units = GetUnitsInRange(coords, 80f, true);
                for (int i = 0; i < units.Count; i++)
                {
                    if (!(units[i].Team == owner.Team || units[i] is IBaseTurret || units[i] is IObjBuilding || units[i] is IInhibitor))
                    {
                        if (units.Count == 1)
                   
                        

                        units[i].TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                    }

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
