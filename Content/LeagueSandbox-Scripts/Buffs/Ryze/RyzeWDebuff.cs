using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;

namespace Buffs
{
    internal class RyzeWDebuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle buff1;
        IParticle buff2;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var caster = ownerSpell.CastInfo.Owner;
            buff1 = AddParticleTarget(caster, unit, "RunePrison_cas.troy", unit);
            buff2 = AddParticleTarget(caster, unit, "RunePrison_tar.troy", unit);
            unit.SetStatus(StatusFlags.CanMove, false);
            unit.SetStatus(StatusFlags.Rooted, true);
            unit.StopMovement();
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(buff1);
            RemoveParticle(buff2);
            unit.SetStatus(StatusFlags.CanMove, true);
            unit.SetStatus(StatusFlags.Rooted, false);

        }

        public void OnUpdate(float diff)
        {

        }
    }
}
