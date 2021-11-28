using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    internal class RyzeRBuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.HEAL;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Ryze_DarkCrystal_Death.troy", unit, buff.Duration);
            AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "manaleach_tar.troy", unit, buff.Duration);
            var vampMod = ownerSpell.CastInfo.SpellLevel;
            if (vampMod.Equals(1))
            {
                StatsModifier.SpellVamp.FlatBonus = (float)(.15);
            }
            else if (vampMod.Equals(2))
            {
                StatsModifier.SpellVamp.FlatBonus = (float)(.20);
            }
            else if (vampMod.Equals(3))
            {
                StatsModifier.SpellVamp.FlatBonus = (float)(.25);
            }
            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {

        }
    }
}
