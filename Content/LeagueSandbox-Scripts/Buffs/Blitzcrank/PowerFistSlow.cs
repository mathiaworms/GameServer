using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using System;
using System.Numerics;

namespace Buffs
{
    internal class PowerFistSlow : IBuffGameScript
    {
        public BuffType BuffType => BuffType.STUN;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_OVERLAPS;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle hitParticle;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var randOffset = (float)new Random().NextDouble();
            var randPoint = new Vector2(unit.Position.X + (80.0f * randOffset), unit.Position.Y + 80.0f * randOffset);

            ForceMovement(unit, "", randPoint, 90.0f, 80.0f, 20.0f, 0.0f, ForceMovementType.FURTHEST_WITHIN_RANGE, ForceMovementOrdersType.CANCEL_ORDER, ForceMovementOrdersFacing.KEEP_CURRENT_FACING);
            buff.SetStatusEffect(StatusFlags.CanAttack | StatusFlags.CanCast | StatusFlags.CanMove, false);
            // ApplyAssistMarker(ownerSpell.CastInfo.Owner, unit, 10.0f);
            hitParticle = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Powerfist_tar.troy", unit, buff.Duration, targetBone: "head", teamOnly: unit.Team, flags: 0);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            buff.SetStatusEffect(StatusFlags.CanMove | StatusFlags.CanAttack | StatusFlags.CanCast, true);
            hitParticle.SetToRemove();
        }

        public void OnUpdate(float diff)
        {

        }
    }
}

