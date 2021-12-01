using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    internal class GalioRighteousGustHaste : IBuffGameScript
    {
        public BuffType BuffType => BuffType.HASTE;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 5;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle p;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            //p = AddParticleTarget(unit, "Overdrive_buf.troy", unit, 1);
            StatsModifier.MoveSpeed.PercentBonus = StatsModifier.MoveSpeed.PercentBonus + (25f + ownerSpell.CastInfo.SpellLevel * 5) / 100f;

            unit.AddStatModifier(StatsModifier);
            p = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "galio_windTunnel_speed_buf.troy", unit, buff.Duration);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);
        }

        public void OnUpdate(float diff)
        {

        }
    }
}

