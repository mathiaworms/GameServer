using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using LeagueSandbox.GameServer.API;

namespace Spells
{
    public class AatroxQ : ISpellScript
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
            var current = new Vector2(spell.CastInfo.Owner.Position.X, spell.CastInfo.Owner.Position.Y);
            FaceDirection(current, spell.CastInfo.Owner, true);
            CreateTimer((float)0.05, () =>
            {
                var trueCoords = GetPointFromUnit(spell.CastInfo.Owner, 750);
                ForceMovement(spell.CastInfo.Owner, "Spell1", trueCoords, 750, 0, 15, 0, GameServerCore.Enums.ForceMovementType.FIRST_WALL_HIT);
                owner.PlayAnimation("Spell1", 1, 0, 1);
            });
        }

        public void OnSpellCast(ISpell spell)
        {
        }
        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var AD = spell.CastInfo.Owner.Stats.AttackDamage.Total * 0.2f;
            var damage = 65 + 35 * (spell.CastInfo.SpellLevel - 1) + AD;
            if (target.Team != owner.Team)
            {
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
            }
        }
        public void OnSpellPostCast(ISpell spell)
        {
            CreateTimer(0.60f, () =>
            {
                var sector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 250f,
                    SingleTick = true,
                    Type = SectorType.Area
                });
            });
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
