using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;

namespace Buffs
{
    internal class KayleE : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle p;
        IObjAiBase owner;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner;
            ApiEventManager.OnHitUnit.AddListener(this, ownerSpell.CastInfo.Owner, TargetExecute, false);
            //unit.AddStatModifier(StatsModifier);
        }
        public void TargetExecute(IAttackableUnit target, bool Iscrit)
        {
            float ap = owner.Stats.AbilityPower.Total * 0.25f;
            float damage = 10 + 10 * owner.GetSpell(1).CastInfo.SpellLevel + ap;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
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
