using GameServerCore.Enums;
using System;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;


namespace Buffs
{
    internal class FizzSeastoneActive : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;
        IObjAiBase Owner;
        float damage;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Owner = ownerSpell.CastInfo.Owner;
            var ap = ownerSpell.CastInfo.Owner.Stats.AbilityPower.Total * 0.30f;
            damage = 5f +( ownerSpell.CastInfo.SpellLevel * 5 ) + ap;
            AddParticleTarget(Owner, Owner, "Fizz_SeastoneTrident.troy", Owner, 5f, bone: "BUFFBONE_GLB_WEAPON_1");
            AddParticleTarget(Owner, Owner, "Fizz_SeastonePassive_Weapon.troy", Owner, bone: "BUFFBONE_GLB_WEAPON_1");

            ApiEventManager.OnHitUnit.AddListener(this, ownerSpell.CastInfo.Owner, TargetTakeDamage, false);

        }

        public void TargetTakeDamage(IAttackableUnit target, bool isCrit)
        {
            target.TakeDamage(Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            ApiEventManager.OnHitUnit.RemoveListener(this);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}

