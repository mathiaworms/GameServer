using System.Numerics;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class GalioRighteousGust : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(ISpell spell)
        {
            AddParticleTarget(spell.CastInfo.Owner, spell.CastInfo.Owner, "galio_windTunnel_mis.troy", spell.CastInfo.Owner, 1f, bone: "L_HAND");
            AddParticleTarget(spell.CastInfo.Owner, spell.CastInfo.Owner, "galio_windTunnel_mis_02.troy", spell.CastInfo.Owner, 1f, bone: "L_HAND");
            AddParticleTarget(spell.CastInfo.Owner, spell.CastInfo.Owner, "galio_windTunnel_mis.troy", spell.CastInfo.Owner, 1f, bone: "L_HAND");
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            var trueCoords = GetPointFromUnit(owner, 1180f);

            SpellCast(owner, 1, SpellSlotType.ExtraSlots, trueCoords, trueCoords, false, Vector2.Zero);
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, ISpellMissile missile)
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
    public class GalioRighteousGustMissile : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            IsDamagingSpell = true
            // TODO
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

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var champion = target as IChampion;
            var owner = spell.CastInfo.Owner as IChampion;
            var spellLevel = owner.GetSpell("GalioRighteousGust").CastInfo.SpellLevel;
            if (champion == null)
            {
                return;
            }

            if (champion.Team == owner.Team && champion != owner)
            {
                AddBuff("GalioRighteousGustHaste", 5f, 1, spell, champion, owner);
                
            }
            else if (champion == owner) //TODO: Fix getting self proc at cast (you are supposed to have to E/Flash into it in order to get the buff i think)
            {
                AddBuff("GalioRighteousGustHaste", 5f, 1, spell, champion, owner);
            }
            else
            {
                var APratio = owner.Stats.AbilityPower.Total * 0.5f;
                var damage = 15 + (45 * spellLevel) + APratio;

                champion.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                
            }
            AddParticleTarget(owner, champion, "galio_windTunnel_unit_tar.troy", champion, lifetime: 1f);
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
