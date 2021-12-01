using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class NamiR : ISpellScript
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
            var owner = spell.CastInfo.Owner;
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            SpellCast(owner, 2, SpellSlotType.ExtraSlots, spellPos, spellPos, false, Vector2.Zero);
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

    public class NamiRMissile : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
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

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            SetStatus(owner, StatusFlags.ForceRenderParticles, true);
            var startPoint = GetPointFromUnit(owner, 145f);
            var endPoint = GetPointFromUnit(owner, 2750f);
            var tempMinion = AddMinion(owner, "TestCubeRender", "TestCubeRender", startPoint, ignoreCollision: true, targetable: false);
            AddBuff("ExpirationTimer", 4.0f, 1, spell, tempMinion, owner);

            // TODO: Vision

            AddParticle(owner, tempMinion, "Nami_Base_R_mis_red.troy", endPoint, bone: "top", lifetime: 2.0f);
            AddParticle(owner, tempMinion, "Nami_Base_R_mis_green.troy", endPoint, bone: "top", lifetime: 2.0f);
            AddParticle(owner, null, "Nami_Base_R_mis.troy", GetPointFromUnit(owner, 2750f));
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            var endPoint = GetPointFromUnit(owner, 2750f);

            spell.CreateSpellSector(new SectorParameters
            {
                BindObject = owner,
                Length = 2750f,
                Width = 500f,
                PolygonVertices = new Vector2[]
                {
                    // Basic square, however the values will be scaled by Length/Width, which will make it a rectangle
                    new Vector2(-1, 0),
                    new Vector2(-1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                },
                SingleTick = true,
                Type = SectorType.Polygon
            });
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;

            var damage =50f + (100f * spell.CastInfo.SpellLevel) + (owner.Stats.AbilityPower.Total * 0.6f);
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL,
                false);
                AddBuff("Stun", 0.5f, 1, spell, target, owner);
            AddParticleTarget(owner, target, "Nami_Base_R_tar.troy", target, 1.0f);
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
