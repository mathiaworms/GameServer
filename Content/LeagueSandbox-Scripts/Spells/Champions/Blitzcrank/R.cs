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
using System.Linq;
using GameServerCore;

namespace Spells
{
    public class StaticField : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
                TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }
        IBuff thisBuff;
        public ISpellSector DamageSector;
        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            var ownerr = spell.CastInfo.Owner as IChampion;
            var spellLevel = ownerr.GetSpell("StaticField").CastInfo.SpellLevel;
            
            var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 1.0f;
            var damage = 125 + spellLevel * 125 + ap;

                AddParticle(owner, null, "StaticField_nova.troy", ownerr.Position, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "StaticField_hit.troy", ownerr.Position, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "StaticField_ready.troy", ownerr.Position, lifetime: 0.5f , reqVision: false);
                DamageSector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 600f,
                    Tickrate = 2,
                    CanHitSameTargetConsecutively = false,
                    OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area,
                    Lifetime = 0.5f
                });
        
            
            
            foreach (var enemy in GetUnitsInRange(ownerr.Position, 600, true)
                .Where(x => x.Team == CustomConvert.GetEnemyTeam(ownerr.Team)))
            {
      
                if (enemy is IObjAiBase)
                {
                    enemy.TakeDamage(ownerr, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

                     AddBuff("Stun", 0.01f, 1, spell, target, ownerr);
                    AddParticleTarget(enemy, enemy, "StaticField_tar.troy", enemy, 1f);
                }
            }
          
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
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
