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
    public class NamiQ : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            CastingBreaksStealth = true,
            DoesntBreakShields = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true,
            SpellToggleSlot = 4
        };
        public ISpellSector DamageSector;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            var spellPos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);
              AddParticle(owner, null, "Nami_Base_Q_pop.troy", spellPos, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "Nami_Base_Q_tar.troy", spellPos, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "Nami_Base_Q_cas.troy", spellPos, lifetime: 0.5f , reqVision: false);
                 AddParticle(owner, null, "Nami_Base_Q_aoe_ground.troy", spellPos, lifetime: 0.5f , reqVision: false);
            DamageSector = spell.CreateSpellSector(new SectorParameters
            {
                Tickrate = 1,
                Length = 160f,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                Type = SectorType.Area
            });
            CreateTimer(1.0f, () => { DamageSector.SetToRemove(); var pre = AddParticle(owner, null, "Nami_Base_Q_aoe_ground.troy", spellPos, lifetime: 4.0f, reqVision: false); });
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var ap = owner.Stats.AbilityPower.Total * 0.5f;
            var damage = 20 + (55 * (spell.CastInfo.SpellLevel)) + ap;
            //Graves_SmokeGrenade_Cloud_Team_Green.troy
            //Graves_SmokeGrenade_Cloud_Team_Red.troy
            target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
              AddBuff("Stun", 1.5f, 1, spell, target, owner);
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
