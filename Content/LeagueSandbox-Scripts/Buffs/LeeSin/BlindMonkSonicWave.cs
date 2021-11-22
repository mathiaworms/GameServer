﻿using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;

namespace BlindMonkQOne
{
    internal class BlindMonkQOne : IBuffGameScript
    {
        public BuffType BuffType => BuffType.HASTE;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public bool IsHidden => false;
        public int MaxStacks => 1;

        public IStatsModifier StatsModifier { get; private set; }

        ISpell originSpell;
        IBuff thisBuff;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            originSpell = ownerSpell;
            thisBuff = buff;
            AddParticleTarget(ownerSpell.CastInfo.Owner, ownerSpell.CastInfo.Owner, "blindMonk_Q_tar_indicator.troy", unit, 1f, buff.Duration);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
            if (thisBuff == null || originSpell == null || thisBuff.Elapsed())
            {
                return;
            }

            var owner = originSpell.CastInfo.Owner;
            var target = thisBuff.TargetUnit;
            if (owner.MovementParameters != null && owner.IsCollidingWith(target))
            {
                owner.SetDashingState(false);
                var ad = owner.Stats.AttackDamage.Total * 1.0f;
                var damage = 50 + (originSpell.CastInfo.SpellLevel * 30) + ad + (0.08f * (target.Stats.HealthPoints.Total - target.Stats.CurrentHealth));
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_REACTIVE, false);
                AddParticleTarget(owner, owner, "GlobalHit_Yellow_tar.troy", target);

                thisBuff.DeactivateBuff();
            }
        }
    }
}