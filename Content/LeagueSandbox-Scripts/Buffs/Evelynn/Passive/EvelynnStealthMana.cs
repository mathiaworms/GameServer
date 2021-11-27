using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    class EvelynnStealthMana : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IBuff thisBuff;
        IChampion champ;
        IParticle p;
        string pEveMana = "evelynnmana.troy";

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var owner = ownerSpell.CastInfo.Owner;
            champ = unit as IChampion;
            thisBuff = buff;

            //Regenerate 1% of maximum mana every second
            StatsModifier.ManaRegeneration.PercentBonus += owner.Stats.ManaPoints.Total * 0.01f;
            champ.AddStatModifier(StatsModifier);

            p = AddParticleTarget(owner, champ, pEveMana, champ, thisBuff.Duration);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);

            champ.RemoveStatModifier(StatsModifier); //Probably not necessary

            if (thisBuff != null)
            {
                thisBuff.DeactivateBuff();
            }
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
