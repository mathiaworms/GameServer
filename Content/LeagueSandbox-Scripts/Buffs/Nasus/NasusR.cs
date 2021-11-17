using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;

namespace Buffs
{
    internal class NasusR : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IObjAiBase owner;
        IParticle p;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner as IChampion;

            var HealthBuff = 150f + 150f * ownerSpell.CastInfo.SpellLevel ;

            p = AddParticleTarget(owner, unit, "Nasus_Base_R_Aura.troy", unit, 1, buff.Duration);
             p = AddParticleTarget(owner, unit, "Nasus_Base_R_Avatar.troy", unit, 1, buff.Duration);
             StatsModifier.Armor.PercentBonus = 25 + 15 * ownerSpell.CastInfo.SpellLevel ;
            StatsModifier.MagicResist.PercentBonus = 25 + 15 * ownerSpell.CastInfo.SpellLevel ;
            StatsModifier.Size.PercentBonus = StatsModifier.Size.PercentBonus + 1;
            StatsModifier.HealthPoints.BaseBonus += HealthBuff;

            unit.AddStatModifier(StatsModifier);
            unit.Stats.CurrentHealth += HealthBuff;
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

