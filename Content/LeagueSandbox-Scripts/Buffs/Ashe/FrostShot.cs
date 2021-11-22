using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain;
namespace Buffs
{
    class FrostShot : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        IParticle p2;
        IObjAiBase Owner;


        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {

                StatsModifier.MoveSpeed.PercentBonus -= 0.1f + (ownerSpell.CastInfo.Owner.GetSpell("FrostShot").CastInfo.SpellLevel * 0.05f);
                 p2 = AddParticleTarget(unit, unit, "iceSparkle_02.troy", unit, buff.Duration);
                unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
             RemoveParticle(p2);
        }

        public void OnPreAttack(ISpell spell)
        {

        }

        public void OnUpdate(float diff)
        {
        }
    }
}
