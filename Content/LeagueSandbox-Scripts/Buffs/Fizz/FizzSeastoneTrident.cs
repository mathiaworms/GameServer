using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;



namespace Buffs
{
    internal class FizzSeastoneTrident : IBuffGameScript
    {

        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_RENEWS;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        float timeSinceLastTick;
        IAttackableUnit Unit;
        float TickingDamage;
        IObjAiBase Owner;
        ISpell spell;
        bool limiter = false;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var owner = ownerSpell.CastInfo.Owner;
            var missinghealth = (unit.Stats.HealthPoints.Total - unit.Stats.CurrentHealth);
            var PercentDamage = (3.5f + (ownerSpell.CastInfo.SpellLevel * 0.5f));
            var AfterDamage = (missinghealth * 0.01f) * PercentDamage;
            var AP = ownerSpell.CastInfo.Owner.Stats.AbilityPower.Total * 0.45f;
            TickingDamage = (30f +  AP + AfterDamage) * 0.33f;
            var damage = AP + AfterDamage;
            Unit = unit;
            Owner = owner;
            spell = ownerSpell;
            limiter = true;

        }
        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {

        }

        public void OnUpdate(float diff)
        {
            timeSinceLastTick += diff;

            if (timeSinceLastTick >= 1000.0f)
            {
                Unit.TakeDamage(Owner, TickingDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLPERSIST, false);
                timeSinceLastTick = 0;
            }
        }
    }
}