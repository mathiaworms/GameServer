using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Domain.GameObjects.Spell.Missile;

namespace Spells
{
    public class YasuoQ2W : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
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
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;

            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            if (owner.HasBuff("YasuoEFIX"))
            {
                CreateTimer(0.45F, () =>
                {
                    owner.PlayAnimation("Spell1E", 0.5f, 0, 1);
                    AddParticleTarget(owner, owner, "Yasuo_Base_EQ_cas.troy", owner);
                    var sector = spell.CreateSpellSector(new SectorParameters
                    {
                        BindObject = spell.CastInfo.Owner,
                        Length = 215f,
                        SingleTick = true,
                        CanHitSameTargetConsecutively = true,
                        OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                        Type = SectorType.Area
                    });
                });
            }
            else
            {
                owner.PlayAnimation("Spell1A", 0.5f, 0, 1);
                CreateTimer(0.15F, () => { AddParticleTarget(owner, owner, "Yasuo_Q_WindStrike.troy", owner); });
                owner.SetStatus(StatusFlags.CanMove, false);
                owner.StopMovement();
                CreateTimer(0.25f, () => { owner.SetStatus(StatusFlags.CanMove, true); });
                CreateTimer(0.25F, () =>
                {
                    var sector = spell.CreateSpellSector(new SectorParameters
                    {
                        BindObject = owner,
                        Length = 450f,
                        Width = 80f,
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
                });
            }
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            AddParticleTarget(owner, target, "Yasuo_Base_Q_hit_tar.troy", target);

            var APratio = owner.Stats.AttackDamage.Total;
            var spelllvl = (spell.CastInfo.SpellLevel * 20);
            target.TakeDamage(owner, APratio / 2 + spelllvl / 2 + 1, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            AddBuff("YasuoQ02", 10.0f, 1, spell, owner, owner);
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