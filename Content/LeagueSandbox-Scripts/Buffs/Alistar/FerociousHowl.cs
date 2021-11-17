using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;


namespace Buffs
{
    internal class FerociousHowl : IBuffGameScript
    {
        public BuffType BuffType => BuffType.HASTE;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var owner = ownerSpell.CastInfo.Owner;

            float AD = 50.0f + 15.0f * ownerSpell.CastInfo.SpellLevel;
            float hpmax = ( 0.8f + 0.2f * ownerSpell.CastInfo.SpellLevel ) * unit.Stats.CurrentHealth;
           

            StatsModifier.Size.PercentBonus = StatsModifier.Size.PercentBonus + 1;
            StatsModifier.AttackDamage.FlatBonus += AD;
            StatsModifier.HealthPoints.FlatBonus += hpmax;
            StatsModifier.Armor.FlatBonus += 100;
            StatsModifier.MagicResist.FlatBonus += 100 ;
            unit.AddStatModifier(StatsModifier);
            //TODO:make damage reduction 

        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
            
        }
    }
}