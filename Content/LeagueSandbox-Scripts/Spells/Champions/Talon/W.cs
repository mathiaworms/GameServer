using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class TalonRake : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            //PlayAnimation(owner, "Spell1");
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            owner.SetStatus(StatusFlags.Targetable, true);
            var Champs = GetChampionsInRange(owner.Position, 50000, true);
            foreach (IChampion player in Champs)
            {
                owner.SetInvisible((int)player.GetPlayerId(), owner, 1f, 0.1f);
                owner.SetHealthbarVisibility((int)player.GetPlayerId(), owner, true);
            }
            AddBuff("TalonDummyBuff", (float)0.75, 1, spell, owner, owner, false);
            for (int bladeCount = 0; bladeCount <= 2; bladeCount++)
             {
                 var targetPos = GetPointFromUnit(owner, 700f, (-13f + (bladeCount * 13f)));
                 SpellCast(owner, 1, SpellSlotType.ExtraSlots, targetPos, targetPos, true, Vector2.Zero);
             }
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

    public class TalonRakeMissileOne : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            IsDamagingSpell = true,
            TriggersSpellCasts = true

            // TODO
        };
        public List<IAttackableUnit> UnitsHit = new List<IAttackableUnit>();

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        private void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile arg3, ISpellSector arg4)
        {
            var owner = spell.CastInfo.Owner;
            var damage = 5f + owner.Stats.AttackDamage.Total * 0.6f + owner.Spells[1].CastInfo.SpellLevel * 25f;

            if (!UnitsHit.Contains(target))
            {
                UnitsHit.Add(target);
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
                AddBuff("TalonWSlow", 2f, 1, spell, target, owner); //TODO: Find Proper Name
                AddParticleTarget(owner, target, "talon_w_tar.troy", target, 1f);
            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }
        public void OnMissileEnd(ISpellMissile missile)
        {
            var owner = missile.CastInfo.Owner;
            if (owner.HasBuff("TalonDummyBuff"))
            {
                SpellCast(owner, 1, SpellSlotType.ExtraSlots, true, owner, missile.Position);
                owner.RemoveBuffsWithName("TalonDummyBuff");
            }
            //SpellCast(owner, 2, SpellSlotType.ExtraSlots, owner.Position, owner.Position, true, missile.Position);
        }
        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            var missile = spell.CreateSpellMissile(new MissileParameters
            {
                Type = MissileType.Circle,
                OverrideEndPosition = end
            });
            ApiEventManager.OnSpellMissileEnd.AddListener(this, missile, OnMissileEnd, true);
            UnitsHit.Clear();
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
