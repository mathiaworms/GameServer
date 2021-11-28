using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;

namespace Buffs
{
    internal class VayneUlt : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle highlander;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {

            var x = (new float[] { 30f, 50f, 70f }[ownerSpell.CastInfo.SpellLevel - 1]);
            LogDebug(x.ToString());
            var owner = ownerSpell.CastInfo.Owner;
            highlander = AddParticleTarget(owner, unit, "VayneInquisition_buf2.troy", unit);

            StatsModifier.MoveSpeed.BaseBonus += 60;
            StatsModifier.AttackDamage.FlatBonus += x;
            unit.AddStatModifier(StatsModifier);
            // TODO: add immunity to slows
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(highlander);
        }

        private void OnAutoAttack(IAttackableUnit target, bool isCrit)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
